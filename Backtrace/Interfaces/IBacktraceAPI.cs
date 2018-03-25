using Backtrace.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Backtrace.Tests")]
namespace Backtrace.Interfaces
{
    /// <summary>
    /// Backtrace API sender interface
    /// </summary>
    /// <typeparam name="T">Attribute type</typeparam>
    public interface IBacktraceApi<T> : IDisposable
    {
        /// <summary>
        /// Send a Backtrace report to Backtrace API
        /// </summary>
        /// <param name="data">Library diagnostic data</param>
        Task<BacktraceServerResponse> Send(BacktraceData<T> data);
    }
}
