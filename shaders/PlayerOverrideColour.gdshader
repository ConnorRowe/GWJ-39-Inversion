shader_type canvas_item;

uniform float override_percent: hint_range(0.0, 1.0) = 0.0;
uniform vec4 override_colour: hint_color = vec4(1.0);

void fragment()
{
	vec4 baseColour = texture(TEXTURE, UV);
	
	COLOR = mix(baseColour, mix(baseColour, override_colour, override_percent), baseColour.a);
}