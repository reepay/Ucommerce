namespace Kruso.Reepay.Extensions.Enums
{
	public enum InvoiceStateEnum
	{
		created = 0,
		pending,        //1
		dunning,        //2
		settled,        //3
		cancelled,      //4
		authorized,     //5
		failed			//6

		//Attention! Add new states below only.
	}
}
