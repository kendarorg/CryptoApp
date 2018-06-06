# CryptoApp

Front End to store KeePass exported xml files.

## Functionalities

* Manage multiple users passwords
* Upload/download standard keepass xml files
* Handle password grouping
* Attach a file to a password
* Administrator users can handle other users access but not to their password files
* The password decrypting passwords are not stored
* The login password are hashed
* Multiple files for each user

## Build

* Go into the ui folder (CryptoApp.Ui)
* Run 'npm install'
* Run 'dist.bat' to copy the data into the website directory
* Open the CryptoApp.sln
* Build the solution

## Installation

* Copy the content of the SQLite folder into the CryptoApp bin folder to add the sqlite.dll native libraries
* Create a Db directory on the web server
* Copy the content of the CryptoUi folder on the server
* Setup the root password
	* Encode a password calling /api/login/enc/[thePassword]
	* Modify the web.config <add key="Admin" value="[THEROOTUSER];[THEENCODEDPASSWORD]" />
* Import the xml export from KeePass
* To update keepass, import with the KeePass XML 1 file format




