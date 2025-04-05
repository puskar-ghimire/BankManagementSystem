public class Transaction
{
    public int TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public int AccountNumber { get; set; }
    public TrnasactionType Type { get; set; }
    public double Amount { get; set; }
    public double BalanceAfterTransaction { get; set; }
    public string TransactionDetail()
    {
        return $"Transaction ID: {TransactionId}, Date: {TransactionDate}, Account Number: {AccountNumber}, Type: {Type}, Amount: {Amount}, Balance After Transaction: {BalanceAfterTransaction}";
    }
}