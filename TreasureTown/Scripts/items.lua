function findPoints(value)
	runCoroutine(function()
		
		if value < 1000 then
			--Play sound!
			showGetGraphic("SmallMoney")
		elseif value < 1500 then
			--Play sound!
			showGetGraphic("Money")
		elseif value >= 1500 then
			--Play sound!
			showGetGraphic("Treasure")
		end
		
		change = 0
		
		if(value > 2500) then
			change = 3
		elseif value >= 1500 then
			change = 2
		elseif value >= 1000 then
			change = 1
		end
		
		waitSeconds(3.5)
		Event.AddChange(change)
		Event.ChangePoints(Event.CurrentTeam(), value)
		waitUntil("Points changed")
		endTurn()
	end)
end

function findBomb()
	runCoroutine(function()
		showGetGraphic("Bomb")
		-- Play sound 
		waitSeconds(3.5)
		-- Show explosion here!
		Event.ChangePoints(Event.CurrentTeam(), Event.GetBombPenalty())
		waitSeconds(0.5)
		Event.AddChange(2)
		showExplosion()
		waitUntil("Points changed")
		endTurn()
	end)
end

function findThief()
	runCoroutine(function()
		showGetGraphic("Thief")
		waitSeconds(3.5)
		Event.Steal(Event.CurrentTeam(), Event.TeamWithHighestPoints())
		
		change = 4
		if(Event.CurrentMode == "Greed") then
			change = 0
		end
		
		Event.AddChange(change)
		waitUntil("Points changed")
		endTurn()
	end)
end

function findMap()
	runCoroutine(function()
		showGetGraphic("Map")
		waitSeconds(3.5)
		Event.SetCurrentTeamMap(true)
		endTurn()
	end)
end

function findEmpty()
	runCoroutine(function()
		print("found nothing")
		endTurn()
	end)
end

function showGetGraphic(name)
	runCoroutine(function()
		Event.CreateGraphic("Graphics/GUI/get"..name, 0, 0, name)
		Event.LerpColor(name, 1, 1, 1, 1, 1)
		waitSeconds(3)
		Event.LerpColor(name, 0,0,0,0,.5)
		waitSeconds(.5)
		Event.DestroyGraphic(name)
		sendSignal("Get graphic shown")
	end)
end