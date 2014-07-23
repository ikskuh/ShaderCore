using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ShaderCore
{
	/// <summary>
	/// The ShaderCore context. Provides methods for creation of shaders,
	/// include files and default shader sources.
	/// It also containts the input layouts supported in the shaders.
	/// </summary>
	public sealed class ShaderContext
	{
		private readonly Dictionary<string, DefaultShader> defaultShaders = new Dictionary<string, DefaultShader>();

		private readonly Dictionary<string, string> includeFiles = new Dictionary<string, string>();

		private readonly Dictionary<string, string> inputLayouts = new Dictionary<string, string>();

		private readonly Thread openGlThread;

		/// <summary>
		/// Fires when the ShaderContext gets a call from the wrong thread into OpenGl.
		/// </summary>
		public event EventHandler<ShaderDeferEventArgs> DeferEvent;

		/// <summary>
		/// Creates a new shader manager.
		/// </summary>
		/// <remarks>Uses the current thread as the OpenGL thread</remarks>
		public ShaderContext()
			: this(Thread.CurrentThread)
		{

		}

		/// <summary>
		/// Creates a new shader manager.
		/// </summary>
		/// <param name="openGlThread">The thread that is the OpenGL thread.</param>
		public ShaderContext(Thread openGlThread)
		{
			this.openGlThread = openGlThread;
		}

		/// <summary>
		/// Creates a new shader.
		/// </summary>
		/// <returns></returns>
		public Shader Create()
		{
			return new Shader(this);
		}

		/// <summary>
		/// Creates a new shader and compiles the given source.
		/// </summary>
		/// <param name="source">Source of the shader.</param>
		/// <returns></returns>
		public Shader Create(string source)
		{
			return this.Create(source, null);
		}

		/// <summary>
		/// Creates a new shader and compiles the given source.
		/// </summary>
		/// <param name="source">Source of the shader.</param>
		/// <param name="tag">Informational tag variable accessible from the shader script.</param>
		/// <returns></returns>
		public Shader Create(string source, object tag)
		{
			var shader = this.Create();
			shader.Load(source, tag);
			return shader;
		}

		/// <summary>
		/// Invokes a method into the OpenGL thread.
		/// </summary>
		/// <param name="method">Method to invoke.</param>
		public void Invoke(Action method)
		{
			if (Thread.CurrentThread == this.openGlThread)
			{
				method();
			}
			else
			{
				if (this.DeferEvent != null)
				{
					this.DeferEvent(this, new ShaderDeferEventArgs(method));
				}
				else
				{
					throw new InvalidOperationException("The invoke can't be executed because the current thread is not the OpenGL thread and there is no event to defer routines.");
				}
			}
		}

		/// <summary>
		/// Logs a line into the system log.
		/// </summary>
		/// <param name="line"></param>
		public void Log(string line)
		{
			if (this.LogMethod == null) return;
			this.LogMethod(line);
		}

		/// <summary>
		/// Logs a formatted line into the system log.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Log(string format, params object[] args)
		{
			this.Log(string.Format(format, args));
		}

		/// <summary>
		/// Checks if the current thread is the OpenGL thread.
		/// </summary>
		internal void ValidateThread()
		{
			if (this.openGlThread != Thread.CurrentThread)
			{
				throw new InvalidOperationException("Invalid thread.");
			}
		}

		/// <summary>
		/// Gets a dictionary of default shaders.
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, DefaultShader> DefaultShaders
		{
			get
			{
				return defaultShaders;
			}
		}

		/// <summary>
		/// Gets a dictionary of include files.
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> IncludeFiles
		{
			get
			{
				return includeFiles;
			}
		}

		/// <summary>
		/// Gets a dictionary of input layouts.
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> InputLayouts
		{
			get
			{
				return inputLayouts;
			}
		}

		/// <summary>
		/// Gets or sets a method that writes a line into the system log.
		/// </summary>
		/// <remarks>By default, this is Console.WriteLine. To have no log at all, set LogMethod to null.</remarks>
		public Action<string> LogMethod { get; set; } = Console.WriteLine;
	}

	/// <summary>
	/// Provides the method that should be deferred into the OpenGL thread.
	/// </summary>
	public class ShaderDeferEventArgs : EventArgs
	{
		private readonly Action method;

		internal ShaderDeferEventArgs(Action method)
		{
			this.method = method;
		}

		/// <summary>
		/// The method to be deferred.
		/// </summary>
		/// <returns></returns>
		public Action Method
		{
			get
			{
				return method;
			}
		}
	}
}
