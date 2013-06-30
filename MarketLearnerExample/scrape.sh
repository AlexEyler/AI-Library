#!/bin/bash

while read line
do
    grep $line 'sp500hst.txt'
done < 'DOW'
