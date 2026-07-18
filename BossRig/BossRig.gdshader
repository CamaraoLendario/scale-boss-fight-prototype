shader_type particles;
uniform vec3[21] BonePoss;
uniform vec3[21] BoneDirs;

vec3 rotateX(vec3 vector, float angle) {
	float cosine = cos(angle);
	float sine = sin(angle);
	
	return vec3(
		vector.x,
		cosine * vector.y - sine * vector.z,
		sine * vector.y + cosine * vector.z
	);
}
vec3 rotateY(vec3 v, float a) {
    float c = cos(a);
    float s = sin(a);

    return vec3(
        c * v.x + s * v.z,
        v.y,
       -s * v.x + c * v.z
    );
}
vec3 rotateZ(vec3 v, float a) {
    float c = cos(a);
    float s = sin(a);

    return vec3(
        c * v.x - s * v.y,
        s * v.x + c * v.y,
        v.z
    );
}
vec3 rotateVec3(vec3 vector, float x, float y, float z) {
	rotateX(vector, x);
	rotateY(vector, y);
	rotateZ(vector, z);
	return vector;
}

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
vec3 RotateToBoneDir(vec3 vec, vec3 boneDir){
	vec3 nBoneDir = normalize(boneDir * vec3(1., 1., -1.));
	vec = rotateZ(vec, atan(nBoneDir.y, sqrt(nBoneDir.x*nBoneDir.x + nBoneDir.z*nBoneDir.z)));
	vec = rotateY(vec, atan(nBoneDir.z, nBoneDir.x));
	return vec;
}
void start() {
	uint alt_seed = hash(INDEX + uint(1.) + 2495087u);
}

void process() {
	uint alt_seed = hash(INDEX + uint(1.) + 2495087u);
	float randF = rand_from_seed(alt_seed);
	int index = int(INDEX)%21;
	//int isUneven = index%2;
	vec3 BonePos = BonePoss[index];
	vec3 BoneDir = BoneDirs[index];
	float boneLen = length(BoneDir);
	//TRANSFORM[3].xyz = BonePos + (BoneDir * ((sin(TIME*5.)*0.5) + .5));
	//TRANSFORM[3].xyz *= 10.;
	
	//float D =
	//BoneDir.x * BonePos.x +
	//BoneDir.y * BonePos.y +
	//BoneDir.z * BonePos.z;
	vec3 pos = vec3(
		randF,
		normalize(vec2(
		cos(PI*2.*fract(randF *587.)),
		sin(PI*2.*fract(randF *157.))))*0.1
	);
	pos *= boneLen;
	//pos = vec3((sin(TIME)+1.)/2., 0., 0.);
	
	pos = RotateToBoneDir(pos, BoneDir);
	
	//TRANSFORM[3].xyz = (BonePos + BoneDir * (sin(TIME*3.)*.5 + .5));
	TRANSFORM[3].xyz = (BonePos + pos)*10.;
}
