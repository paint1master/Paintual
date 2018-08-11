// EngineCpp.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <amp.h>

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



