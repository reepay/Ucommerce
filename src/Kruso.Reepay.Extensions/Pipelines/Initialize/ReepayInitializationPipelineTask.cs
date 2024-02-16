using System;
using System.Linq;
using Kruso.Reepay.Extensions.Services.Interfaces;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace Kruso.Reepay.Extensions.Pipelines.Initialize
{
    public class ReepayInitializationPipelineTask : IPipelineTask<InitializeArgs>
    {
        private readonly IReepayLogger<ReepayInitializationPipelineTask> _logger;

        public ReepayInitializationPipelineTask(IReepayLogger<ReepayInitializationPipelineTask> logger)
        {
            _logger = logger;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            var paymentMethodDefinitionName = "Reepay"; //Must be equal to id in Payments.config
            var paymentMethodName = paymentMethodDefinitionName;

            var definition =
                Definition.All()
                    .FirstOrDefault(d => d.DefinitionType.DefinitionTypeId == 4 && d.Name == paymentMethodDefinitionName);
            if (definition != null)
            {
                definition.Deleted = false;
            }
            else
            {
                definition = new Definition
                {
                    Name = paymentMethodDefinitionName,
                    Description = "Configuration for Reepay",
                    DefinitionType = DefinitionType.Get(4),
                };
                _logger.LogInfo("Reepay payment definition created.");
            }

            var definitionfields = definition.DefinitionFields;
            if (definitionfields != null)
            {
                CreateOrUpdateDefinitionField(definition, "Accept_url", "ShortText");
                CreateOrUpdateDefinitionField(definition, "Accept_url_change_cc", "ShortText");
                CreateOrUpdateDefinitionField(definition, "Error_url", "ShortText");
                CreateOrUpdateDefinitionField(definition, "Cancel_url", "ShortText");
                CreateOrUpdateDefinitionField(definition, "Cancel_url_my_account", "ShortText");
                CreateOrUpdateDefinitionField(definition, "Button_text_for_subscription", "ShortText");
                CreateOrUpdateDefinitionField(definition, "ReepayPrivateKey", "ShortText");
                CreateOrUpdateDefinitionField(definition, "Test", "Boolean");

                definition.Save();
            }

            return PipelineExecutionResult.Success;
        }

        private void CreateOrUpdateDefinitionField(Definition definition, string name, string dataType, string defaultValue = "")
        {
            if (definition == null) throw new ArgumentNullException("definition");

            var definitionfields = definition.DefinitionFields;
            if (!definitionfields.Any(x => x.Name == name && x.Definition.DefinitionId == definition.DefinitionId))
            {
                CreateAndAddDefinitionField(definition, name, false, true, true, defaultValue, dataType);
            }
            else
            {
                var definitionfield = definitionfields.FirstOrDefault(x => x.Name == name && x.Definition.DefinitionId == definition.DefinitionId);
                UpdateDefinitionField(definitionfield, false, true, true, defaultValue, dataType, false);
            }
        }

        private void CreateAndAddDefinitionField(Definition definition, string name, bool multilingual, bool displayOnSite, bool rederInEditor, string defaultValue, string dataType)
        {
            definition.AddDefinitionField(new DefinitionField
            {
                //If the money should be withdrawn instantly, e.g download
                Name = name,
                Multilingual = multilingual,
                DisplayOnSite = displayOnSite,
                RenderInEditor = rederInEditor,
                DefaultValue = defaultValue,
                DataType = DataType.FirstOrDefault(x => x.TypeName.Equals(dataType)),
            });
        }

        public void UpdateDefinitionField(DefinitionField definitionField, bool multilingual, bool displayOnSite, bool rederInEditor, string defaultValue, string dataType, bool deleted)
        {
            definitionField.Multilingual = multilingual;
            definitionField.Deleted = deleted;
            definitionField.DisplayOnSite = displayOnSite;
            definitionField.RenderInEditor = rederInEditor;
            definitionField.DefaultValue = defaultValue;
            definitionField.DataType = DataType.FirstOrDefault(x => x.TypeName.Equals(dataType));
        }
    }
}
