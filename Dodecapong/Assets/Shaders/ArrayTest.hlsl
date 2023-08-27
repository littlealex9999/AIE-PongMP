#ifndef ARRAYTEST_INCLUDED
#define ARRAYTEST_INCLUDED

float _testArray[256];
int _testArraySize = 256;

void GetArrayVal_float(int index, out float val)
{
	val = _testArray[index];
}

#endif