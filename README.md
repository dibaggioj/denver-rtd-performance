# Denver RTD Bus Allocation Performance Reporter

## IWKS 4120/5120 Lab 6

Nanu Ahluwalia
John DiBaggio
Kyle Etsler

April 6, 2017
 
Project modified from JKB test-transit

### About

Outputs an hourly report based on data of each route on a minute basis: its average timeliness (how many seconds late or early its buses are on average) and its total number of buses (trips) over the hour duration. It does not output any results for a route when there are no trips along that route during the hour duration. For example:

###################################################################################
Outputting hourly route bus timeliness for 2017/04/05 from 21:27:03 to 22:27:03
Route: 0, average time: -113.5 seconds, buses: 8, average status: LATE
Route: 1, average time: -189.33 seconds, buses: 3, average status: LATE
Route: 10, average time: -60 seconds, buses: 2, average status: LATE
Route: 100, average time: -134.33 seconds, buses: 3, average status: LATE
Route: 105, average time: -79 seconds, buses: 3, average status: LATE
Route: 11, average time: -8 seconds, buses: 4, average status: LATE
Route: 112, average time: 71 seconds, buses: 1, average status: EARLY
Route: 12, average time: 40.5 seconds, buses: 4, average status: EARLY
Route: 120, average time: 63 seconds, buses: 1, average status: EARLY
Route: 121, average time: -34 seconds, buses: 3, average status: LATE
Route: 130, average time: 7 seconds, buses: 2, average status: EARLY
Route: 133, average time: -54 seconds, buses: 1, average status: LATE
Route: 135, average time: 43 seconds, buses: 1, average status: EARLY
Route: 139, average time: -123 seconds, buses: 1, average status: LATE
Route: 15, average time: -190.6 seconds, buses: 10, average status: LATE
Route: 153, average time: -5.25 seconds, buses: 4, average status: LATE
Route: 15L, average time: -65.25 seconds, buses: 4, average status: LATE
Route: 16, average time: -225.14 seconds, buses: 7, average status: LATE
Route: 169, average time: -56.5 seconds, buses: 2, average status: LATE
Route: 169L, average time: 32 seconds, buses: 1, average status: EARLY
Route: 19, average time: -116 seconds, buses: 2, average status: LATE
Route: 20, average time: -39 seconds, buses: 1, average status: LATE
Route: 205, average time: -269.5 seconds, buses: 2, average status: LATE
Route: 21, average time: -175 seconds, buses: 4, average status: LATE
Route: 225, average time: 72 seconds, buses: 1, average status: EARLY
Route: 24, average time: 66 seconds, buses: 1, average status: EARLY
Route: 27, average time: -30 seconds, buses: 1, average status: LATE
Route: 28, average time: -74 seconds, buses: 1, average status: LATE
Route: 3, average time: -42 seconds, buses: 1, average status: LATE
Route: 31, average time: -106.33 seconds, buses: 3, average status: LATE
Route: 36, average time: -161 seconds, buses: 2, average status: LATE
Route: 38, average time: -692.33 seconds, buses: 3, average status: LATE
Route: 40, average time: -25.25 seconds, buses: 4, average status: LATE
Route: 42, average time: -63 seconds, buses: 4, average status: LATE
Route: 43, average time: 10 seconds, buses: 2, average status: EARLY
Route: 44, average time: 7.33 seconds, buses: 3, average status: EARLY
Route: 45, average time: -329.67 seconds, buses: 3, average status: LATE
Route: 48, average time: 387 seconds, buses: 1, average status: EARLY
Route: 51, average time: -85.5 seconds, buses: 2, average status: LATE
Route: 52, average time: -72 seconds, buses: 1, average status: LATE
Route: 6, average time: -119 seconds, buses: 1, average status: LATE
Route: 65, average time: -105 seconds, buses: 1, average status: LATE
Route: 66, average time: -45.33 seconds, buses: 3, average status: LATE
Route: 73, average time: -85.5 seconds, buses: 2, average status: LATE
Route: 76, average time: -111.33 seconds, buses: 3, average status: LATE
Route: 83L, average time: -59.25 seconds, buses: 4, average status: LATE
Route: 88, average time: -139 seconds, buses: 3, average status: LATE
Route: 92, average time: -5 seconds, buses: 1, average status: LATE
Route: AA, average time: -86.5 seconds, buses: 2, average status: LATE
Route: AB, average time: -11.33 seconds, buses: 3, average status: LATE
Route: AT, average time: -6 seconds, buses: 1, average status: LATE
Route: BOLT, average time: -135 seconds, buses: 2, average status: LATE
Route: BOND, average time: -573.5 seconds, buses: 2, average status: LATE
Route: DASH, average time: -179 seconds, buses: 2, average status: LATE
Route: FF1, average time: -87.17 seconds, buses: 6, average status: LATE
Route: JUMP, average time: 9 seconds, buses: 2, average status: EARLY
Route: LX, average time: 0 seconds, buses: 1, average status: EARLY
Route: N, average time: -153 seconds, buses: 1, average status: LATE
Route: SKIP, average time: -262.5 seconds, buses: 4, average status: LATE
###################################################################################
