## Convert DataReader to PSObject
This repo is for a class library used in the SimplySql PowerShell Module.
It enables the use of the -stream switch on Invoke-SqlQuery,
it turns the output of DataReader into PSObject with just members
mapped from the columns.

Attempted to duplicate this code in native PowerShell resulted in a
performance penalty of 3.5x over not using the switch.

