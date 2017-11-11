using System;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a type that can provide RemoteFieldValues.
    /// </summary>
    public interface IRemoteIssueFieldProvider
    {
        Task<RemoteFieldValue[]> GetRemoteFieldValuesAsync(CancellationToken token);
    }
}
