﻿// ECMAScript 5 strict mode
"use strict";

assert2(cr, "cr namespace not created");
assert2(cr.plugins_, "cr.plugins_ not created");
/// Global objects and function 
var GameHandler = {}
GameHandler.setUserInfo = function (user){
	GameHandler.userInfo = user;
}

GameHandler.playerInfos = [];

//  Test region
var user = {
	msgEvent: "JOIN_OR_CREATE_ROOM_CLIENT_to_SERVER",
	playerName: 'chipl_2',
	avatarUrl: "something.png",
	playerJwt: "aabbcccderfd",
	game: "Tala",
	playerRoomId: 1,
	playerRoomName: "Meocon"
};



GameHandler.setUserInfo(user);

/////////////////////////////////////
// Plugin class
// *** CHANGE THE PLUGIN ID HERE *** - must match the "id" property in edittime.js
//          vvvvvvvv
cr.plugins_.GameTaLaPlugin = function(runtime)
{
	this.runtime = runtime;
};

(function ()
{
	/////////////////////////////////////
	// *** CHANGE THE PLUGIN ID HERE *** - must match the "id" property in edittime.js
	//                            vvvvvvvv
	var pluginProto = cr.plugins_.GameTaLaPlugin.prototype;
		
	/////////////////////////////////////
	// Object type class
	pluginProto.Type = function(plugin)
	{
		this.plugin = plugin;
		this.runtime = plugin.runtime;
	};

	var typeProto = pluginProto.Type.prototype;

	// called on startup for each object type
	typeProto.onCreate = function()
	{
	};

	/////////////////////////////////////
	// Instance class
	pluginProto.Instance = function(type)
	{
		this.type = type;
		this.runtime = type.runtime;
		
		// any other properties you need, e.g...
		// this.myValue = 0;
	};
	
	var instanceProto = pluginProto.Instance.prototype;

	var userInfo = {};

	
	// called whenever an instance is created
	instanceProto.onCreate = function()
	{
		// note the object is sealed after this call; ensure any properties you'll ever need are set on the object
		// e.g...
		// this.myValue = 0;		
		// Chipl Code Start here
		this.userInfo = GameHandler.userInfo;
		// console.log('userInfo:', this.userInfo)
		// Chipl Code End here
	};
	
	// called whenever an instance is destroyed
	// note the runtime may keep the object after this call for recycling; be sure
	// to release/recycle/reset any references to other objects in this function.
	instanceProto.onDestroy = function ()
	{
	};
	
	// called when saving the full state of the game
	instanceProto.saveToJSON = function ()
	{
		// return a Javascript object containing information about your object's state
		// note you MUST use double-quote syntax (e.g. "property": value) to prevent
		// Closure Compiler renaming and breaking the save format
		return {
			// e.g.
			//"myValue": this.myValue
		};
	};
	
	// called when loading the full state of the game
	instanceProto.loadFromJSON = function (o)
	{
		// load from the state previously saved by saveToJSON
		// 'o' provides the same object that you saved, e.g.
		// this.myValue = o["myValue"];
		// note you MUST use double-quote syntax (e.g. o["property"]) to prevent
		// Closure Compiler renaming and breaking the save format
	};
	
	// only called if a layout object - draw to a canvas 2D context
	instanceProto.draw = function(ctx)
	{
	};
	
	// only called if a layout object in WebGL mode - draw to the WebGL context
	// 'glw' is not a WebGL context, it's a wrapper - you can find its methods in GLWrap.js in the install
	// directory or just copy what other plugins do.
	instanceProto.drawGL = function (glw)
	{
	};
	
	// The comments around these functions ensure they are removed when exporting, since the
	// debugger code is no longer relevant after publishing.
	/**BEGIN-PREVIEWONLY**/
	instanceProto.getDebuggerValues = function (propsections)
	{
		// Append to propsections any debugger sections you want to appear.
		// Each section is an object with two members: "title" and "properties".
		// "properties" is an array of individual debugger properties to display
		// with their name and value, and some other optional settings.
		propsections.push({
			"title": "My debugger section",
			"properties": [
				// Each property entry can use the following values:
				// "name" (required): name of the property (must be unique within this section)
				// "value" (required): a boolean, number or string for the value
				// "html" (optional, default false): set to true to interpret the name and value
				//									 as HTML strings rather than simple plain text
				// "readonly" (optional, default false): set to true to disable editing the property
				
				// Example:
				// {"name": "My property", "value": this.myValue}
			]
		});
	};
	
	instanceProto.onDebugValueEdited = function (header, name, value)
	{
		// Called when a non-readonly property has been edited in the debugger. Usually you only
		// will need 'name' (the property name) and 'value', but you can also use 'header' (the
		// header title for the section) to distinguish properties with the same name.
		if (name === "My property")
			this.myProperty = value;
	};
	/**END-PREVIEWONLY**/

	//////////////////////////////////////
	// Conditions
	function Cnds() {};

	// the example condition
	Cnds.prototype.MyCondition = function (myparam)
	{
		// return true if number is positive
		return myparam >= 0;
	};
	
	// ... other conditions here ...
	Cnds.prototype.OnOpened = function ()
	{
		return true;
	};
	
	Cnds.prototype.OnClosed = function ()
	{
		return true;
	};
	
	Cnds.prototype.OnError = function ()
	{
		return true;
	};
	
	Cnds.prototype.OnMessage = function ()
	{
		return true;
	};
	
	Cnds.prototype.IsOpen = function ()
	{
		return this.ws && this.ws.readyState === 1 /* OPEN */;
	};
	
	Cnds.prototype.IsConnecting = function ()
	{
		return this.ws && this.ws.readyState === 0 /* CONNECTING */;
	};
	
	Cnds.prototype.IsSupported = function ()
	{
		return isSupported;
	};	

	Cnds.prototype.NewPlayerJoin = function ()
	{
		return true;
	};	

	//PlayerLeft
	Cnds.prototype.PlayerLeft = function ()
	{
		return true;
	};	

	// CheckPlayer
	Cnds.prototype.CheckPlayer = function (index){
		return GameHandler.playerInfos.find(x=>x.post == index);
	}


	pluginProto.cnds = new Cnds();
	
	//////////////////////////////////////
	// Actions
	function Acts() {};

	// the example action
	Acts.prototype.MyAction = function (myparam)
	{
		// alert the message
		alert(myparam);
	};

	// Connect to Server
	Acts.prototype.Connect = function (ws_url, protocol){
		
		if (this.ws){
			this.ws.close();
		}
		var self = this;
		var webSocket = window.WebSocket || window.MozWebSocket;
		this.ws = new webSocket(ws_url);
		this.ws.onopen = function (e) {        	
			do {
			}
			while (self.ws.readyState !== 1);
			// self.ws.send("Iam here")	;
			// Chipl code
			self.runtime.trigger(cr.plugins_.GameTaLaPlugin.prototype.cnds.OnOpened,self);
			
			if (self.userInfo){
				self.ws.send( JSON.stringify(self.userInfo));
			}
			
        }

        this.ws.onclose = function (e) {
            self.runtime.trigger(cr.plugins_.GameTaLaPlugin.prototype.cnds.OnClosed,self);
        }

        this.ws.onmessage = function (e) {
                //var dataObject = JSON.parse(e.data);
                //console.log(dataObject);
                // console.log("Event: ",e);
                console.log(e.data);
				// ws.send(" I am here.. available");
				var player = JSON.parse(e.data);
				var serverEventType = player.msgEvent;
				switch(serverEventType){
					case "JOIN_OR_CREATE_ROOM_SERVER_to_CLIENT":
						if (GameHandler.playerInfos.indexOf((player) != -1)){
							GameHandler.playerInfos.push(player);
						}
						self.runtime.trigger(cr.plugins_.GameTaLaPlugin.prototype.cnds.NewPlayerJoin,self);
						break;
					case "PLAYER_LEFT_ROOM_SERVER_to_CLIENT":
						GameHandler.playerInfos = GameHandler.playerInfos.filter(item=>{
							return item != player;
						})
						self.runtime.trigger(cr.plugins_.GameTaLaPlugin.prototype.cnds.PlayerLeft,self);
						break;
				}
        }

	}

	Acts.prototype.Close = function ()
	{
		if (this.ws)
			this.ws.close();
	};
	
	Acts.prototype.Send = function (msg_)
	{		
		if (!this.ws || this.ws.readyState !== 1 /* OPEN */){			
			return;
		}			
		this.ws.send(msg_);
	};
	
	
	// ... other actions here ...
	
	pluginProto.acts = new Acts();
	
	//////////////////////////////////////
	// Expressions
	function Exps() {};
	
	// the example expression
	Exps.prototype.MyExpression = function (ret)	// 'ret' must always be the first parameter - always return the expression's result through it!
	{
		ret.set_int(1337);				// return our value
		// ret.set_float(0.5);			// for returning floats
		// ret.set_string("Hello");		// for ef_return_string
		// ret.set_any("woo");			// for ef_return_any, accepts either a number or string
	};

	// Other
	Exps.prototype.OtherExpression = function(ret, a,b){
		ret.set_string("hello");
	}
	//
	Exps.prototype.SpecialExpression = (ret, a, b)=>{
		ret.set_float(a + b);
	}
	
	// ... other expressions here ...
	
	pluginProto.exps = new Exps();

}());