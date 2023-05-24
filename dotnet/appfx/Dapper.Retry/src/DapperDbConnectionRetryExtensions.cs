using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;

using Dapper;

using Polly.Retry;

namespace Bearz.Dapper.Retry;

[SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together")]
public static class DapperDbConnectionRetryExtensions
{
    /// <summary>
    /// Execute parameterized SQL.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>The number of rows affected.</returns>
    public static int RetryExecute(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.Execute(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Execute parameterized SQL.
    /// </summary>
    /// <param name="cnn">The connection to execute on.</param>
    /// <param name="command">The command to execute on this connection.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>The number of rows affected.</returns>
    public static int RetryExecute(this IDbConnection cnn, CommandDefinition command, RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.Execute(command));
    }

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <param name="cnn">The connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>The first cell selected as <see cref="object"/>.</returns>
    public static object RetryExecuteScalar(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.ExecuteScalar(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">The connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>The first cell returned, as <typeparamref name="T"/>.</returns>
    public static T RetryExecuteScalar<T>(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <param name="cnn">The connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>The first cell selected as <see cref="object"/>.</returns>
    public static object RetryExecuteScalar(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.ExecuteScalar(command));
    }

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">The connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>The first cell selected as <typeparamref name="T"/>.</returns>
    public static T RetryExecuteScalar<T>(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.ExecuteScalar<T>(command));
    }

    /// <summary>
    /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
    /// </summary>
    /// <param name="cnn">The connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
    /// or <see cref="T:DataSet"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// DataTable table = new DataTable("MyTable");
    /// using (var reader = ExecuteReader(cnn, sql, param))
    /// {
    ///     table.Load(reader);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public static IDataReader RetryExecuteReader(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.ExecuteReader(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
    /// </summary>
    /// <param name="cnn">The connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
    /// or <see cref="T:DataSet"/>.
    /// </remarks>
    public static IDataReader RetryExecuteReader(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.ExecuteReader(command));
    }

    /// <summary>
    /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
    /// </summary>
    /// <param name="cnn">The connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="commandBehavior">The <see cref="CommandBehavior"/> flags for this reader.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
    /// or <see cref="T:DataSet"/>.
    /// </remarks>
    public static IDataReader RetryExecuteReader(
        this IDbConnection cnn,
        CommandDefinition command,
        CommandBehavior commandBehavior,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.ExecuteReader(command, commandBehavior));
    }

    /// <summary>
    /// Return a sequence of dynamic objects with properties matching the columns.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;.</remarks>
    /// <returns>Dynamic data from the query.</returns>
    public static IEnumerable<dynamic> RetryQuery(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        bool buffered = true,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.Query(sql, param, transaction, buffered, commandTimeout, commandType));
    }

    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;.</remarks>
    /// <returns>Dynamic data from the query.</returns>
    public static dynamic RetryQueryFirst(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryFirst(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;.</remarks>
    /// <returns>Dynamic data from the query.</returns>
    public static dynamic RetryQueryFirstOrDefault(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;.</remarks>
    /// <returns>Dynamic data from the query.</returns>
    public static dynamic RetryQuerySingle(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QuerySingle(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;.</remarks>
    /// <returns>Dynamic data from the query.</returns>
    public static dynamic RetryQuerySingleOrDefault(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QuerySingleOrDefault(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="buffered">Whether to buffer results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static IEnumerable<T> RetryQuery<T>(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        bool buffered = true,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T RetryQueryFirst<T>(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T RetryQueryFirstOrDefault<T>(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T RetryQuerySingle<T>(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T RetryQuerySingleOrDefault<T>(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="buffered">Whether to buffer results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static IEnumerable<object> RetryQuery(
        this IDbConnection cnn,
        Type type,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        bool buffered = true,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.Query(type, sql, param, transaction, buffered, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static object RetryQueryFirst(
        this IDbConnection cnn,
        Type type,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryFirst(type, sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static object RetryQueryFirstOrDefault(
        this IDbConnection cnn,
        Type type,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryFirstOrDefault(type, sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static object RetryQuerySingle(
        this IDbConnection cnn,
        Type type,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QuerySingle(type, sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static object RetryQuerySingleOrDefault(
        this IDbConnection cnn,
        Type type,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QuerySingleOrDefault(type, sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static IEnumerable<T> RetryQuery<T>(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.Query<T>(command));
    }

    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A single instance or null of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T RetryQueryFirst<T>(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryFirst<T>(command));
    }

    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A single or null instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T RetryQueryFirstOrDefault<T>(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryFirstOrDefault<T>(command));
    }

    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A single instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T RetryQuerySingle<T>(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QuerySingle<T>(command));
    }

    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>
    /// A single instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T RetryQuerySingleOrDefault<T>(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QuerySingleOrDefault<T>(command));
    }

    /// <summary>
    /// Execute a command that returns multiple result sets, and access each in turn.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>A grid reader.</returns>
    public static SqlMapper.GridReader RetryQueryMultiple(
        this IDbConnection cnn,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryMultiple(sql, param, transaction, commandTimeout, commandType));
    }

    /// <summary>
    /// Execute a command that returns multiple result sets, and access each in turn.
    /// </summary>
    /// <param name="cnn">The connection to query on.</param>
    /// <param name="command">The command to execute for this query.</param>
    /// <param name="retryPolicy">The policy for the number of retries.</param>
    /// <returns>A grid reader.</returns>
    public static SqlMapper.GridReader RetryQueryMultiple(
        this IDbConnection cnn,
        CommandDefinition command,
        RetryPolicy? retryPolicy = null)
    {
        retryPolicy ??= DapperPollyOptions.DefaultRetryPolicy;
        return retryPolicy.Execute(() => cnn.QueryMultiple(command));
    }
}