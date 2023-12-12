# Gosho File System

A file system simulator, supporting both a GUI and Unix-like command line interface.
It implements the following commands: 


* create {sectorSize}\* {sectorSizeUnit}\* {maxFilesystemSizeKb}\*          Creates the file system if it does not exist
* mkdir {dirName}                                                        Creates a directory
* tree                                                                   Prints the entire file system tree
* ls {dirName}\*                                                          Lists the contents of a directorCreates a directory
* rmdir {dirName}                                                        Removes a directory
* cd {dirName}                                                           Changes the current directory
* rm {fileName}                                                          Removes the file
* write +append\* {file} ""data""                                       Creates a file if it does not exists and writes the data
* import +append\* {source} {destination}                                 Creates a file if it does not exists and writes the data
* export {source} {destination}                                          Creates a file if it does not exists and writes the data


The program supports data deduplication and checks the consistency of the data through a custom hashing alghorithm.
All used data structures have been implemented from scratch.
