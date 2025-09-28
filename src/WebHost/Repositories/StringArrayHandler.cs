using Dapper;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace WebHost.Repositories;

public class StringArrayHandler : SqlMapper.TypeHandler<string[]>
{
    public override void SetValue(IDbDataParameter parameter, string[] value)
    {
        parameter.Value = value;
        if (parameter is NpgsqlParameter npgsqlParam)
            npgsqlParam.NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Text;
    }

    public override string[] Parse(object value)
    {
        return (value as string[]) ?? Array.Empty<string>();
    }
}
