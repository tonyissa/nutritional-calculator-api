using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace nutritional_calculator_api.Helpers;

public class SqliteFKInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        if (command.CommandText.StartsWith("PRAGMA foreign_keys"))
        {
            command.CommandText = "PRAGMA foreign_keys=ON;";
        }

        return base.NonQueryExecuting(command, eventData, result);
    }
}
