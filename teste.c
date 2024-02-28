#include <stdio.h>
#include <stdlib.h>

void main (void){

  int x = 45;
  int *y;
  y = &x;

  printf("Y aponta para: %p\n", y);
}
