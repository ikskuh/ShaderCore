using System.Collections.Generic;

namespace ShaderCore
{
	/// <summary>
	/// Defines a default shader that can be included via shader:addDefault(name)
	/// </summary>
	public sealed class DefaultShader
	{
		/// <summary>
		/// Gets or sets a set of include files needed for this default shader.
		/// The files will be included for the global shader file.
		/// </summary>
		/// <returns></returns>
		public ISet<string> Includes { get; set; } = new HashSet<string>();

		/// <summary>
		/// Gets or sets the source code of this default shader.
		/// </summary>
		/// <returns></returns>
		public string Source { get; set; } = "";

		/// <summary>
		/// Gets or sets the type of the default shader.
		/// All types passable to shader:add{type=?} are valid.
		/// </summary>
		/// <returns></returns>
		public string ShaderType { get; set; } = "";
	}
}