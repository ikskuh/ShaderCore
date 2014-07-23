shader.version = "400 core";
shader:add {
	type = "global",
	source = [[
		uniform vec4 pixelColor;
	]]
}
shader:add {
	type = "vertex",
	source = [[
		layout(location = 0) in vec3 vPos;
	
		void main() {
			gl_Position.xyz = vPos;
			gl_Position.w = 1.0;
		}
	]]}

shader:add {
	type = "fragment",
	source = [[
		layout(location = 0) out vec4 color;
		void main() {
			color = pixelColor;
		}
	]]
}