﻿using Backtrace.Base;
using Backtrace.Interfaces;
using Backtrace.Model;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Backtrace
{
    /// <summary>
    /// Backtrace .NET Client 
    /// </summary>
    public class BacktraceClient : Backtrace<object>, IBacktraceClient<object>
    {
        /// <summary>
        /// Set an event executed when send function triggers
        /// </summary>
        public Action<BacktraceReport<object>> OnReportStart;

        /// <summary>
        /// Set an event executed after data send to Backtrace API
        /// </summary>
        public Action<BacktraceReport<object>> AfterSend;

        /// <summary>
        /// Initialize Backtrace report client
        /// </summary>
        /// <param name="sectionName">Backtrace configuration section in App.config or Web.config file. Default section is BacktraceCredentials</param>
        /// <param name="attributes">Attributes scoped for every report</param>
        /// <param name="databaseDirectory">Database path</param>
        /// <param name="reportPerMin">Numbers of records send per one min</param>
        public BacktraceClient(
                string sectionName = "BacktraceCredentials",
                Dictionary<string, object> attributes = null,
                string databaseDirectory = "",
                uint reportPerMin = 3
            )
            : base(BacktraceCredentials.ReadConfigurationSection(sectionName),
                  attributes, databaseDirectory, reportPerMin)
        {
        }

        /// <summary>
        /// Initialize client with BacktraceCredentials
        /// </summary>
        /// <param name="backtraceCredentials">Backtrace credentials to access Backtrace API</param>
        /// <param name="attributes">Attributes scoped for every report</param>
        /// <param name="databaseDirectory">Database path</param>
        /// <param name="reportPerMin">Numbers of records send per one minute</param>
        public BacktraceClient(
            BacktraceCredentials backtraceCredentials,
            Dictionary<string, object> attributes = null,
            string databaseDirectory = "",
            uint reportPerMin = 3)
            : base(backtraceCredentials, attributes, databaseDirectory, reportPerMin)
        { }

        /// <summary>
        /// Send a backtrace report to Backtrace API
        /// </summary>
        /// <param name="backtraceReport">report</param>
        public new bool Send(BacktraceReport<object> backtraceReport)
        {
            OnReportStart?.Invoke(backtraceReport);
            var result = base.Send(backtraceReport);
            AfterSend?.Invoke(backtraceReport);
            return result;
        }

        /// <summary>
        /// Send an exception to Backtrace API
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="attributes">Additional information about application state</param>
        /// <param name="attachmentPaths">Path to all report attachments</param>
        public virtual void Send(
            Exception exception,
            Dictionary<string, object> attributes = null,
            List<string> attachmentPaths = null)
        {
            var report = new BacktraceReport<object>(exception, attributes, attachmentPaths)
            {
                CallingAssembly = Assembly.GetCallingAssembly()
            };
            Send(report);
        }
        /// <summary>
        /// Send a message to Backtrace API
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="attributes">Additional information about application state</param>
        /// <param name="attachmentPaths">Path to all report attachments</param>
        public virtual bool Send(
            string message,
            Dictionary<string, object> attributes = null,
            List<string> attachmentPaths = null)
        {
            var report = new BacktraceReport<object>(message, attributes, attachmentPaths)
            {
                CallingAssembly = Assembly.GetCallingAssembly()
            };
            return Send(report);
        }
    }
}
