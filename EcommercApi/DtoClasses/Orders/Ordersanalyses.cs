namespace EcommercApi.DtoClasses;

public class Ordersanalyses
{
    public int Totalorders { get; set; }
    public double ConfirmedPercentage { get; set; }
    public double DeliveredPercentage { get; set; }
    public double ReturnPercentage { get; set; }
    public double PendingPercentage { get; set; }
}