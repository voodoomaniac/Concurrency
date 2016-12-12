using System;
using System.Data;

namespace Concurrency.DAL.Extensions
{
    public static class DbCommandExtensions
    {
        public static void AddWithValue(this IDbCommand command, string name, object value)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}