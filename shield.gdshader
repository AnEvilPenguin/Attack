shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}

uniform vec4 color1 : source_color;
uniform bool exposed;

void fragment() {
	// Called for every pixel the material is visible on.
	// COLOR = vec4(1.0, 1.0, 1.0, 1.0); // For every pixel output white
	
	vec4 input_color = texture(TEXTURE, UV); // Grab the colour of the texture TEXTURE at the position UV
	
	// .r = red, .b = blue, .g = green, .a = alpha
	// COLOR = vec4(input_color.r, input_color.b, input_color.g, input_color.a);
	
	if (input_color.g > 0.7 && !exposed) {
		COLOR = vec4(0.5, 0.5, 0.5, 1.0);
	}
	else if (input_color.a == 0.0 || input_color.g > 0.7) {
		COLOR = input_color;
	}
	else {
		// Shift colour in direction of given color
		COLOR = input_color * color1;
	}
	
	//if (input_color.r == 0.411764705882353 && input_color.b == 0.415686274509804 && input_color.g == 0.415686274509804) {
//		COLOR = color1;
//	} else if (input_color.r > 0.3) {
		//COLOR = color2;
	//} else if (input_color.r > 0.2) {
//		COLOR = color3;
//	}
	
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
