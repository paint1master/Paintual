// EngineCpp.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <amp.h>
using namespace concurrency;

extern "C"             //No name mangling
__declspec(dllexport)  //Tells the compiler to export the function
int                    //Function return type     
__cdecl                //Specifies calling convention, cdelc is default, 
					   //so this can be omitted 
	testIncrement(int number) {
	return number + 1;
}

extern "C" __declspec(dllexport) double __cdecl 
	testMult(double a, double b) {
	return a * b;
}

extern "C" __declspec(dllexport) double __cdecl
testMultArray(double d[]) {
	double result = d[0] * d[1];
	return result;
}

extern "C" __declspec(dllexport) void __cdecl
angle(int n, double* divSpread255_array, double* divSpread2f_array, double* angle_array, double* lum_array) {
	array_view<double, 1> av_divSpread255(n, divSpread255_array);
	array_view<double, 1> av_divSpread2f(n, divSpread2f_array);
	array_view<double, 1> av_angle(n, angle_array);
	array_view<double, 1> av_lum(n, lum_array);

	parallel_for_each(av_angle.extent, [=](index<1> idx) restrict(amp)
	{
		av_angle[idx] = av_lum[idx] * av_divSpread255[idx] - av_divSpread2f[idx];
	});
}



