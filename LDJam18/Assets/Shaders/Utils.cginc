#define PI				3.1415926535897932384626433832795
#define TWO_PI			6.283185307179586476925286766559 
#define DIVIDE_PI		0.31830988618379067153776752674503
#define DIVIDE_TWO_PI	0.15915494309189533576888376337251
#define EPSILON			0.0000000000000000000000000000000000001

float remap(float fromMin, float fromMax, float toMin, float toMax, float value) {
	return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
}
