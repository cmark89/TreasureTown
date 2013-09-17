---------------------
----scheduler.lua----
---------------------

--Stores all couroutines that are waiting for their wake up time
local WAITING_FOR_TIME = {}
-- Stores all couroutines that are waiting for a signal to resume
local WAITING_FOR_SIGNAL = {}
--The current game time.
local CURRENT_TIME = 0

local SENT_SIGNALS = {}

--Wrap the method of creating a coroutine
function runCoroutine(func)
	local co = coroutine.create(func)
	return coroutine.resume(co)
end


function waitSeconds(sec)
	--Grab the current running coroutine
	local this_routine = coroutine.running()
	if(this_routine ~= nil) then
		--Find the time to resume the coroutine
		local resumeTime = CURRENT_TIME + sec
		
		--Store the coroutine and resume time in the table
		WAITING_FOR_TIME[this_routine] = resumeTime
		--Suspend
		return coroutine.yield(this_routine)
	end
end


function waitUntil(sig)
	--Grab the current coroutine
	local co = coroutine.running()
	if(co ~= nil) then
		--Store the coroutine in the table with its signal
		WAITING_FOR_SIGNAL[co] = sig
		--Suspend
		coroutine.yield(co)
	end
end


function sendSignal(sig)
	SENT_SIGNALS[sig] = 1
end
	
	
function updateCoroutines(deltaTime)
	
	--Update current time
	CURRENT_TIME = CURRENT_TIME + deltaTime
	
	--Create a list of coroutines to awaken
	local time_resume_list = {}
	local signal_resume_list = {}
	
	--Check each suspended routine to see if it should resume this frame
	for co, t in pairs(WAITING_FOR_TIME) do
		if(CURRENT_TIME >= t) then
			table.insert(time_resume_list, co)
		end
	end
	
	--Check each signal against waiting coroutines
	for co, sig in pairs(WAITING_FOR_SIGNAL) do
		if(SENT_SIGNALS[sig] ~= nil) then
			table.insert(signal_resume_list, co)
		end
	end
	
	--Awake the resumeList
	for i, co in ipairs(time_resume_list) do
		--Remove the coroutine from the table
		WAITING_FOR_TIME[co] = nil
		--Resume the coroutine
		coroutine.resume(co)
	end
	
	for i, co in ipairs(signal_resume_list) do
		--Remove the coroutine from the table
		WAITING_FOR_SIGNAL[co] = nil
		--Resume the coroutine
		coroutine.resume(co)
	end
	
	--Finally, remove all fired signals
	SENT_SIGNALS = {}
end


--Used to remove all coroutines
function abortAllCoroutines()
	SENT_SIGNALS = {}
	WAITING_FOR_SIGNAL = {}
	WAITING_FOR_TIME = {}
end