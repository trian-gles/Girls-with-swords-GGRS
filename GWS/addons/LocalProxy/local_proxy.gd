extends Node

class_name UDPProxy


var relay := PacketPeerUDP.new()
var local := PacketPeerUDP.new()

var local_client_ip = ""
var local_client_port = -1

var running = false


func start_proxy(room_code: String, relay_host: String, relay_port: int, local_port: int) -> bool:
	var err = local.listen(local_port)

	if err != OK:
		push_error("Failed to listen on local UDP port %d" % local_port)
		return false

	err = relay.connect_to_host(relay_host, relay_port)

	if err != OK:
		push_error("Failed to connect to relay %s:%d" % [relay_host, relay_port])
		return false

	relay.put_packet(room_code.to_utf8())

	running = true
	set_process(true)
	
	print("UDP proxy started")
	print("Local port: %d" % local_port)
	print("Relay: %s:%d" % [relay_host, relay_port])
	print("Room: %s" % room_code)

	return true


func stop_proxy():
	print("Closing proxy")
	running = false
	set_process(false)

	local.close()
	relay.close()

	local_client_ip = ""
	local_client_port = -1

func _exit_tree():
	stop_proxy()


func _process(_delta):
	if not running:
		return

	#
	# Local application -> Relay
	#
	while local.get_available_packet_count() > 0:
		var packet = local.get_packet()

		# Remember who sent the packet locally
		local_client_ip = local.get_packet_ip()
		local_client_port = local.get_packet_port()

		relay.put_packet(packet)


	#
	# Relay -> Local application
	#
	while relay.get_available_packet_count() > 0:
		var packet = relay.get_packet()

		if local_client_port != -1:
			local.set_dest_address(
				local_client_ip,
				local_client_port
			)

			local.put_packet(packet)
