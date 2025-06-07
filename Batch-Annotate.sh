#!/bin/bash

# --- Configuration ---
# This script will run its main action loop continuously until you press Ctrl+C.

# Ensure ydotoold is running independently with the correct socket configuration.
# Example command to run in *another* terminal (or before this script):
# sudo killall ydotoold; sleep 1; sudo ydotoold --socket-path="$XDG_RUNTIME_DIR/myscript.ydotool" --socket-own="$(id -u):$(id -g)" &

export YDOTOOL_SOCKET="$XDG_RUNTIME_DIR/myscript.ydotool" # Socket for ydotool client
YDTOOL_CMD="ydotool"

WAIT_TIME_SHORT="0.5" # Delay between individual key actions
WAIT_TIME_MEDIUM="0.7" # Longer delay if needed
LOOP_DELAY="0.3"       # Delay in seconds between each full sequence of actions

# Keycodes (VERIFY THESE WITH 'sudo libinput record' on YOUR system!)
KC_ENTER="96"     # Defaulting to Numpad Enter. Use 28 for main Enter key if preferred and verified.
KC_LCTRL="29"     # KEY_LEFTCTRL (usually stable)
KC_RIGHT="106"    # COMMON VALUE for Right Arrow. PLEASE VERIFY THIS for your KEY_RIGHT with 'libinput record'.

# --- Trap Ctrl+C (SIGINT) ---
# This function is called when you press Ctrl+C in the terminal.
cleanup_and_exit() {
    echo "" # Newline after ^C
    echo "Ctrl+C pressed. Stopping script."
    # Attempt to release any stuck modifiers (e.g., if Ctrl was down when Ctrl+C was hit)
    $YDTOOL_CMD key "$KC_LCTRL:0" 2>/dev/null # Release Left Ctrl
    exit 0 # This command terminates the script and breaks the infinite loop.
}
# Tell the script to call 'cleanup_and_exit' when Ctrl+C (INT signal) is received.
trap cleanup_and_exit INT

# --- Helper function to check ydotoold daemon ---
# This is run once before the main loop starts.
check_ydotoold_daemon() {
    echo "DEBUG: Using YDOTOOL_SOCKET=$YDOTOOL_SOCKET"
    echo "DEBUG: Checking if ydotoold daemon process is running..."

    if ! pgrep -x ydotoold > /dev/null; then
        echo "ERROR: ydotoold daemon process NOT found!"
        echo "       Please ensure it's running AND was started with the correct socket path."
        echo "       Required command (run in another terminal or before this script):"
        echo "       sudo ydotoold --socket-path=\"$YDOTOOL_SOCKET\" --socket-own=\"$(id -u):$(id -g)\" &"
        exit 1 # Exit the script if ydotoold is not set up.
    fi
    echo "DEBUG: ydotoold daemon process found (pgrep check)."

    if [ ! -S "$YDOTOOL_SOCKET" ]; then
        echo "ERROR: ydotoold process might be running, but the socket file '$YDOTOOL_SOCKET' was NOT found!"
        echo "       Ensure ydotoold was started with: --socket-path=\"$YDOTOOL_SOCKET\""
        exit 1
    fi
    echo "DEBUG: Socket file '$YDOTOOL_SOCKET' found."

    if ! which $YDTOOL_CMD > /dev/null; then
        echo "ERROR: $YDTOOL_CMD client command not found. Please install it (e.g., sudo pacman -S ydotool)."
        exit 1
    fi
    echo "DEBUG: $YDTOOL_CMD client path is $(which $YDTOOL_CMD)"
}

# --- Main Script Logic ---
echo "Script starting. It will simulate key presses REPEATEDLY."
echo "The actions will loop until you press Ctrl+C in this terminal."
echo "---------------------------------------------------------------------"
echo "IMPORTANT: Ensure ydotoold is running with the correct configuration:"
echo "sudo ydotoold --socket-path=\"$YDOTOOL_SOCKET\" --socket-own=\"\$(id -u):\$(id -g)\" &"
echo "---------------------------------------------------------------------"

# Perform prerequisite check ONCE before the loop.
check_ydotoold_daemon

echo "DEBUG: WAYLAND_DISPLAY is $WAYLAND_DISPLAY"
echo "DEBUG: User is $(whoami)"
echo "DEBUG: Current keyboard layout (approximate, from setxkbmap if X11):"
setxkbmap -query 2>/dev/null | grep layout || echo " (Could not determine from setxkbmap, or not Wayland/X11, or setxkbmap not installed)"

echo "Switch to your target window in 5 seconds... The actions will then loop."
sleep 5

# --- Infinite Loop for Actions ---
loop_count=0
while true; do  # This loop will run forever
    loop_count=$((loop_count + 1))
    echo "" # Blank line for readability
    echo "--- Starting action sequence #$loop_count ---"

    echo "Action 1: Typing '1' (using 'ydotool type \"!\"')..."
    $YDTOOL_CMD type "@" #! = 1 en 2=@
    sleep "$WAIT_TIME_SHORT"

    echo "Action 2: Pressing 'Enter' (keycode $KC_ENTER)..."
    $YDTOOL_CMD key "$KC_ENTER:1" "$KC_ENTER:0"
    sleep "$WAIT_TIME_SHORT"

    echo "Action 3: Pressing 'Ctrl + Right Arrow' (keycodes $KC_LCTRL, $KC_RIGHT)..."
    $YDTOOL_CMD key "$KC_LCTRL:1"      # Press Left Ctrl down
    sleep 0.1                         # Tiny delay
    $YDTOOL_CMD key "$KC_RIGHT:1" "$KC_RIGHT:0" # Press Right Arrow down & up
    sleep 0.1                         # Tiny delay
    $YDTOOL_CMD key "$KC_LCTRL:0"      # Release Left Ctrl
    sleep "$WAIT_TIME_MEDIUM"

    echo "--- Action sequence #$loop_count finished. Waiting $LOOP_DELAY seconds before next loop. ---"
    sleep "$LOOP_DELAY" # Pause before repeating the loop
done # End of the 'while true' loop

# This part of the script should ideally not be reached if Ctrl+C is used,
# as 'exit 0' in 'cleanup_and_exit' will terminate the script.
echo "Script loop ended (this is unexpected if using Ctrl+C)."
exit 0
