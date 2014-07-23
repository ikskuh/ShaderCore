using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCore
{
	/// <summary>
	/// Defines an OpenGL shader object.
	/// </summary>
	public sealed class ShaderFragment
	{
		private readonly Shader parent;
		int id;
		ShaderType type;

		/// <summary>
		/// Creates a new shader fragment.
		/// </summary>
		/// <param name="parent">The parent shader of this fragment.</param>
		/// <param name="type">Type of the shader fragment.</param>
		internal ShaderFragment(Shader parent, ShaderType type)
		{
			this.parent = parent;
			this.type = type;
			parent.Manager.Invoke(() => this.id = GL.CreateShader(type));
		}

		/// <summary>
		/// Creates a new shader fragment and compiles it.
		/// </summary>
		/// <param name="parent">The parent shader of this fragment.</param>
		/// <param name="type">Type of the shader fragment.</param>
		/// <param name="source">Source code to compile.</param>
		internal ShaderFragment(Shader parent, ShaderType type, string source)
			: this(parent, type)
		{
			this.Compile(source);
		}

		/// <summary>
		/// Fails.
		/// </summary>
		public void Bind()
		{
			throw new InvalidOperationException("Cannot bind an OpenGL shader object.");
		}

		/// <summary>
		/// Compiles the shader fragment.
		/// </summary>
		/// <param name="sources">Source code parts to compile.</param>
		public void Compile(params string[] sources)
		{
			this.parent.Manager.Invoke(() =>
				{
					GL.ShaderSource(this.id, sources.Length, sources, (int[])null);
					GL.CompileShader(this.id);

					int status;
					string infoLog = GL.GetShaderInfoLog(this.id);
					GL.GetShader(this.id, ShaderParameter.CompileStatus, out status);
					if (status == 0)
					{
						this.parent.Manager.Log(LocalizedStrings.Default.ShaderCompilerFailed, this.type);
						if (!string.IsNullOrWhiteSpace(infoLog))
						{
							this.parent.Manager.Log(infoLog);
						}
					}
					else
					{
						if (!string.IsNullOrWhiteSpace(infoLog))
						{
							this.parent.Manager.Log(LocalizedStrings.Default.ShaderCompilerResult, this.type);
							this.parent.Manager.Log(infoLog);
						}
					}
				});
		}
		/// <summary>
		/// Destroys the shader.
		/// </summary>
		public void Dispose()
		{
			if (this.id != 0)
			{
				GL.DeleteShader(this.id);
				this.id = 0;
			}
		}

		/// <summary>
		/// The OpenGL shader object id.
		/// </summary>
		public int Id { get { return this.id; } }

		/// <summary>
		/// Gets the parent shader for this shader fragment.
		/// </summary>
		/// <returns></returns>
		public Shader Parent
		{
			get
			{
				return parent;
			}
		}

		/// <summary>
		/// Gets the type of this shader fragment.
		/// </summary>
		public ShaderType Type
		{
			get { return type; }
		}
	}
}
