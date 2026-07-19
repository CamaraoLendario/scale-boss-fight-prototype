shader_type particles;

uniform float radius = 1.5;
uniform bool exploding = false;

uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}
float rand_from_seed(inout uint seed) {
	int k;
	int s = int(seed);
	if (s == 0) {
		s = 305420679;
	}
	k = s / 127773;
	s = 16807 * (s - k * 127773) - 2836 * k;
	if (s < 0) {
		s += 2147483647;
	}
	seed = uint(s);
	return float(seed % uint(65536)) / 65535.0;
}
void start() {
	uint alt_seed = hash(INDEX + uint(1.) + RANDOM_SEED);
	float frand = rand_from_seed(alt_seed);
	USERDATA2.xyz = vec3(
		rand_from_seed(alt_seed) * PI * 2.,
		rand_from_seed(alt_seed) * PI * 2.,
		rand_from_seed(alt_seed) * PI * 2.
	);
	
	vec2[3] trigs = {
		vec2(cos(USERDATA2.x), sin(USERDATA2.x)),
		vec2(cos(USERDATA2.y), sin(USERDATA2.y)),
		vec2(cos(USERDATA2.z), sin(USERDATA2.z))
	};
	
	TRANSFORM[0].xyz = vec3(
		trigs[1].x * trigs[2].x,
		trigs[1].x * trigs[2].y,
		-trigs[1].y
	);
	TRANSFORM[1].xyz = vec3(
		trigs[0].y * trigs[1].y * trigs[2].x - trigs[0].x * trigs[2].y,
		trigs[0].y * trigs[1].y * trigs[2].y + trigs[0].x * trigs[2].x,
		trigs[0].y * trigs[1].x
	);
	TRANSFORM[2].xyz = vec3(
		trigs[0].x * trigs[1].y * trigs[2].x + trigs[0].y * trigs[2].y,
		trigs[0].x * trigs[1].y * trigs[2].y - trigs[0].y * trigs[2].x,
		trigs[0].x * trigs[1].x
	);
	USERDATA1.w = 0.;
}

void process() {
	uint alt_seed = hash(INDEX + uint(1.) + RANDOM_SEED);
	float frand = rand_from_seed(alt_seed);
	
	if (exploding)
		USERDATA1.w += DELTA/(LIFETIME*.5);
	
	TRANSFORM[3].xyz = normalize(vec3(
		2.*fract(frand*321.537)-1.,
		2.*fract(frand*712.568)-1.,
		2.*fract(frand*198.563)-1.
	))*radius * (frand*.5 +.5 + USERDATA1.w *10.);
	
	float height = sin(USERDATA1.w * PI) *2.;
	
	TRANSFORM[3].xyz += vec3(
		TRANSFORM[3].x * USERDATA1.w,
		height,
		TRANSFORM[3].z * USERDATA1.w
	);
	if (USERDATA1.w > frand){
		TRANSFORM[3].xyz = vec3(999.);
	}
	
	USERDATA2.xyz += vec3(
		USERDATA1.w * 0.1,
		USERDATA1.w * 0.1,
		USERDATA1.w * 0.1
	);
	
	vec2[3] trigs = {
		vec2(cos(USERDATA2.x), sin(USERDATA2.x)),
		vec2(cos(USERDATA2.y), sin(USERDATA2.y)),
		vec2(cos(USERDATA2.z), sin(USERDATA2.z))
	};
	
	TRANSFORM[0].xyz = vec3(
		trigs[1].x * trigs[2].x,
		trigs[1].x * trigs[2].y,
		-trigs[1].y
	);
	TRANSFORM[1].xyz = vec3(
		trigs[0].y * trigs[1].y * trigs[2].x - trigs[0].x * trigs[2].y,
		trigs[0].y * trigs[1].y * trigs[2].y + trigs[0].x * trigs[2].x,
		trigs[0].y * trigs[1].x
	);
	TRANSFORM[2].xyz = vec3(
		trigs[0].x * trigs[1].y * trigs[2].x + trigs[0].y * trigs[2].y,
		trigs[0].x * trigs[1].y * trigs[2].y - trigs[0].y * trigs[2].x,
		trigs[0].x * trigs[1].x
	);
}
