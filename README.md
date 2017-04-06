# Denver RTD Bus Allocation Performance Reporter

## IWKS 4120/5120 Lab 6

Nanu Ahluwalia  
John DiBaggio  
Kyle Etsler  

April 6, 2017  
 
Project modified from JKB test-transit  

### About

Outputs an hourly report based on data of each  route on a minute basis: its average timeliness (how many seconds late or early its buses are on average) and its total number of buses (trips) over the hour duration. The results are ordered from latest to earliest. It does not output any results for a route when there are no trips along that route during the hour duration. For example:  

###################################################################################  
Outputting hourly route bus timeliness for 2017/04/05 from 22:09:04 to 23:09:04  

Route: BOND, average time: -575.67 seconds, buses: 3, average status: LATE  
Route: 38, average time: -437.5 seconds, buses: 2, average status: LATE  
Route: 11, average time: -271.67 seconds, buses: 3, average status: LATE  
Route: 52, average time: -231 seconds, buses: 1, average status: LATE  
Route: 51, average time: -224.5 seconds, buses: 2, average status: LATE  
Route: 29, average time: -219.5 seconds, buses: 2, average status: LATE  
Route: 42, average time: -211 seconds, buses: 4, average status: LATE  
Route: 15, average time: -193.25 seconds, buses: 8, average status: LATE  
Route: 43, average time: -191.67 seconds, buses: 3, average status: LATE  
Route: 27, average time: -190 seconds, buses: 1, average status: LATE  
Route: 73, average time: -187 seconds, buses: 1, average status: LATE  
Route: JUMP, average time: -182 seconds, buses: 1, average status: LATE  
Route: 169L, average time: -180 seconds, buses: 1, average status: LATE  
Route: 153, average time: -168.25 seconds, buses: 4, average status: LATE  
Route: 21, average time: -162 seconds, buses: 3, average status: LATE  
Route: 65, average time: -161 seconds, buses: 1, average status: LATE  
Route: 15L, average time: -155.8 seconds, buses: 5, average status: LATE  
Route: 31, average time: -149.75 seconds, buses: 4, average status: LATE  
Route: 44, average time: -147 seconds, buses: 3, average status: LATE  
Route: 76, average time: -144 seconds, buses: 1, average status: LATE  
Route: FF1, average time: -117 seconds, buses: 5, average status: LATE  
Route: 105, average time: -114 seconds, buses: 3, average status: LATE  
Route: 130, average time: -107 seconds, buses: 1, average status: LATE  
Route: AA, average time: -98.5 seconds, buses: 2, average status: LATE  
Route: 45, average time: -96 seconds, buses: 3, average status: LATE  
Route: 48, average time: -95 seconds, buses: 2, average status: LATE  
Route: SKIP, average time: -92.5 seconds, buses: 2, average status: LATE  
Route: 3, average time: -85.5 seconds, buses: 2, average status: LATE  
Route: 169, average time: -62 seconds, buses: 2, average status: LATE  
Route: 0, average time: -58.2 seconds, buses: 10, average status: LATE  
Route: BOLT, average time: -54 seconds, buses: 2, average status: LATE  
Route: 205, average time: -52 seconds, buses: 1, average status: LATE  
Route: 121, average time: -51.25 seconds, buses: 4, average status: LATE  
Route: 40, average time: -48.25 seconds, buses: 4, average status: LATE  
Route: 6, average time: -29.5 seconds, buses: 2, average status: LATE  
Route: 88, average time: -20 seconds, buses: 2, average status: LATE  
Route: 16, average time: -5 seconds, buses: 6, average status: LATE  
Route: N, average time: -3 seconds, buses: 1, average status: LATE  
Route: 83L, average time: -1.5 seconds, buses: 4, average status: LATE  
Route: 10, average time: -1 seconds, buses: 3, average status: LATE  
Route: FF3, average time: 0 seconds, buses: 1, average status: EARLY  
Route: 12, average time: 9.67 seconds, buses: 3, average status: EARLY  
Route: 66, average time: 37 seconds, buses: 1, average status: EARLY  
Route: 1, average time: 74 seconds, buses: 1, average status: EARLY  
Route: 100, average time: 94 seconds, buses: 1, average status: EARLY  
Route: 120X, average time: 205 seconds, buses: 1, average status: EARLY  
Route: AT, average time: 300 seconds, buses: 1, average status: EARLY  
Route: 92, average time: 301 seconds, buses: 1, average status: EARLY  
Route: AB, average time: 304.67 seconds, buses: 3, average status: EARLY  
Route: LX, average time: 598 seconds, buses: 1, average status: EARLY  
###################################################################################  
