using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WebDX.Api
{
    /// <summary>
    /// Common plugin interface.
    /// </summary>
    public interface ICommonPlugin
    {
        /// <summary>
        /// Called when a session is starting in the client.
        /// </summary>
        void StartSession(Scripts.StartWorkEventArgs e);
        /// <summary>
        /// Called when a session is ending in the client.
        /// </summary>
        void EndSession(Scripts.EndWorkEventArgs e);
        /// <summary>
        /// Called when the current document changes in the client.
        /// </summary>
        void StartDocument();
        /// <summary>
        /// Called when the current document is about to change in the client.
        /// </summary>
        void EndDocument();
        /// <summary>
        /// Called when a field becomes active in the client.
        /// </summary>
        void StartField();
        /// <summary>
        /// Called when a field is about to become inactive in the client.
        /// </summary>
        void EndField();
    }

    /// <summary>
    /// The session plugin interface for interactive plugins.
    /// </summary>
	public interface ISessionPlugin : ICommonPlugin
    {
        /// <summary>
        /// The parent control used to doc the plugin UI.
        /// </summary>
        System.Windows.Forms.Control Parent { get; set;}
    }

	/// <summary>
	/// Extended session plugin interface
	/// </summary>
    public interface ISessionPlugin_R1 : ISessionPlugin
	{
		/// <summary>
		/// Called when the current document is about to change in the client.
		/// </summary>
		/// <param name="e">The exit event arguments</param>
		void EndDocument(Scripts.ExitEventArgs e);
		/// <summary>
		/// Called when a field is about to become inactive in the client.
		/// </summary>
		/// <param name="e">The exit event arguments</param>
		void EndField(Scripts.ExitEventArgs e);
	}

    /// <summary>
    /// The client plugin interface for background plugins.
    /// </summary>
	public interface IClientPlugin : ICommonPlugin
    {
        /// <summary>
        /// Called to initialize the plugin.
        /// </summary>
        /// <param name="serverUrl">The URL to the launching directory.</param>
        /// <param name="query">The start up query string.</param>
        void Init(string serverUrl, string query);
        /// <summary>
        /// Called when the client is shutting down.
        /// </summary>
        void Shutdown();
    }

	/// <summary>
	/// Extended client plugin interface
	/// </summary>
	public interface IClientPlugin_R1 : IClientPlugin
	{
		/// <summary>
		/// Called when the current document is about to change in the client.
		/// </summary>
		/// <param name="e">The exit event arguments</param>
		void EndDocument(Scripts.ExitEventArgs e);
		/// <summary>
		/// Called when a field is about to become inactive in the client.
		/// </summary>
		/// <param name="e">The exit event arguments</param>
		void EndField(Scripts.ExitEventArgs e);
	}
}
