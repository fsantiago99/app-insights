using System.Linq.Expressions;
using AppInsights.Domain.Entities;

namespace AppInsights.Application.Laps;

internal static class ValidLapSpecification
{
    public static Expression<Func<Lap, bool>> IsValid { get; } =
        lap => !lap.IsDeleted && lap.IsAccurate && lap.TrackStatus == "1";

    public static Expression<Func<Lap, bool>> IsValidForTheoreticalBest { get; } =
        lap => lap.IsAccurate && lap.TrackStatus == "1";
}
