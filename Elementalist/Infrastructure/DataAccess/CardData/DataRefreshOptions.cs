namespace Elementalist.Infrastructure.DataAccess.CardData;

public class DataRefreshOptions
{
    private double _hours;

    /// <summary>
    /// Time to refresh in hours. Gaurenteed to be a positive value.
    /// </summary>
    public double Hours { get => _hours; init => _hours = (value >= 0) ? value : -value; }
}
