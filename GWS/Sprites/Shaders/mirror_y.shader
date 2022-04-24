shader_type canvas_item;

uniform vec4 color :hint_color;
uniform float reflection_intensity = 0.5;
uniform sampler2D transition_gradient :hint_black;

// Updated from GDScript
uniform float scale_y = 1.0f;
uniform float zoom_y = 1.0f;

void fragment() {
	float uv_size_ratio_v = SCREEN_PIXEL_SIZE.y / TEXTURE_PIXEL_SIZE.y;
	vec2 uv_reflected = vec2(SCREEN_UV.x, SCREEN_UV.y + uv_size_ratio_v * UV.y * 2.0 * scale_y * zoom_y);

	vec4 reflection_color = texture(SCREEN_TEXTURE, uv_reflected);
	float transition = texture(transition_gradient, vec2(1.0 - UV.y, 1.0)).r;
	COLOR = mix(color, reflection_color, transition * reflection_intensity);
}