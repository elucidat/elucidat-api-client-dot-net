Elucidat API : Example .NET client
==================================

Prerequisites
-------------

This solution requires Visual Studio 2013 and .NET framework 4 or higher.

Configuring and running the example
-----------------------------------

You will need to set the following information in the <appSettings> section of Demo/App.config before
building the solution:

* publicKey - your Elucidat public API key
* secretKey - your Elucidat secret API key
* baseUrl - the base url for API calls, usually https://api.elucidat.com/v2/
* projectCode - the code (id) of a project under your account, ideally one with releases
* releaseCode - the code (id) of a release under your account
* authorEmail - the email of an author under your account

If all of this information is not set correctly, some or all of the API calls in the demo
program may fail.

Once built, you can run the example program by opening a command prompt in the
Demo\bin\Debug folder, and running Demo.exe. The program will pause and wait for you
to press 'Enter' after each API call to allow viewing the output. 
