using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CreateRefundRequest
	{
		[JsonProperty("invoice")]
		public string Invoice { get; set; }

		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("note_lines")]
		public IList<CreateCreditNoteLine> Note_lines { get; set; }

		[JsonProperty("manual_transfer")]
		public ManualTransfer Manual_transfer { get; set; }
	}
}
