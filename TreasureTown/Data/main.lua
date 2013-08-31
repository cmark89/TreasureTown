--This is the main game loop, using a modified scheduler.

gameFinished = false
teams = { }
shiftCounter = 6
shiftCount = 0
shiftMode = nil
possibleShiftModes = { "Danger", "Crime", "Greed", "English" }

-- This must be run as a coroutine
function mainGameLoop()
	
	--Setup the game
	initialGameSetup()
	
	while not gameFinished do

		--Set the team whose turn is next and begin the turn
		currentTeam = getNextTeam()
		startTurn(currentTeam);

		--Suspend the main loop
		sleepUntil(currentTeam.Name.." turn finished.")

		--Check to see if the conditions for game over have been met
		checkGameOver()
		if not gameFinished then
			-- If the game hasn't ended, see if the game needs to shift
			checkShift()
			sleepUntil("Shift check finished.")
		end
		
	end

	--Go to the results screen
	goToResults()
end



function getNextTeam()

	-- Shuffle the teams around return the highest.

end


function checkGameOver()

	if treasureFound or timeOver or earlyAbort then

		gameFinished = true;

	else

		gameFinished = false;

	end

end


function initialGameSetup()

	--Pass the shift mode to the randomization function and get a new town
	randomizeTown(shiftMode)
	
	-- Setup the teams by putting them in their proper order and giving them lives
	--for loop
		teams[i].Points = 0
		teams[i].Lives = 3
		teams[i].HasMap = false

end


function checkShift()
	
	if(shiftCounter <= 0) then
		shiftCounter = 6
		startShift()
		sleepUntil("Map shifted.")	
	end
	sendSignal("Shift check finished.")
end

function startShift()

	--shiftMode = possibleShiftModes[random...]	
	shiftCount = shiftCount + 1
	randomizeTown(shiftMode, shiftCount)
	showShiftWarning(shiftMode)
	waitSeconds(4)
	sendSignal("Map shifted.")
end

function showShiftWarning(shiftMode)
	shift = Game:CreateGraphicElement("Graphics/GUI/"..shiftMode, -600, 250)
	shift.LerpPosition(0,250, 2)
	waitSeconds(4)
	shift.LerpPosition(600, 250, 1.5)
	waitSeconds(1.5)
	shift.Destroy()
end