from PIL import Image
from collections import deque
import json


# Pixel patterns for each tile type (down, right, up, left neighbors)
elementsArrays = [
    [0,0,220,220],
    [220,0,220,0],
    [220,220,220,0],
    [220,220,220,220],
    [220, 0, 0, 0],
]

# Human-readable names matching each pattern above
elementsNames = [
    "Corner",
    "Corridor",
    "T-split",
    "Open",
    "Dead end",
]

# Output JSON structure holding all identified map elements
map = {
    "mapElements": [],
}

def main():
    # Open the bitmap, get its dimensions
    img = Image.open("C:\\Users\\Joop_\\Manpack\\Assets\\Devtools\\basic.bmp")
    width, height = img.size
    # Loop through each pixel
    for y in range(height - 2):
        for x in range(width - 2):
            pixel = img.getpixel((x + 1, y + 1))
            if(pixel == 220): # 220 = walkable/open tile
                instance = {
                    "name": "",
                    "prefab": 0,
                    "rotationX": 0,
                    "rotationY": 0,
                    "rotationZ": 0,
                    "locationX": 0,
                    "locationY": 0,
                    "locationZ": 0,

                }
                # Sample the four cardinal neighbors (down, right, up, left)
                openSides = [
                    img.getpixel((x + 1, y + 2)),
                    img.getpixel((x + 2, y + 1)),
                    img.getpixel((x + 1, y)),
                    img.getpixel((x, y + 1)),
                ]

                
                # Rotate the pattern up to 4 times to match a known element type
                for i in range(4):
                    d = deque(openSides)
                    d.rotate(i)
                    if([*d] in elementsArrays):
                        # Set tile type, rotation, and world position
                        instance["name"] = elementsNames[elementsArrays.index([*d])]
                        instance["prefab"] = elementsArrays.index([*d])
                        instance["rotationY"] = (i * 90 - 90) % 360
                        instance["locationX"] = x + 1
                        instance["locationZ"] = y + 1
                        break
                
                # Add to map
                map["mapElements"].append(instance)
                
    # Write the resulting map data to a JSON file
    with open('C:\\Users\\Joop_\\Manpack\\Assets\\Devtools\\map_debug.json', 'w') as fp:
        print("test")
        json.dump(map, fp)

if __name__ == '__main__':
    main()

# Sources
    # https://stackoverflow.com/questions/5773397/converting-a-deque-object-into-list
    # https://www.w3schools.com/python/python_dictionaries.asp
    # https://www.reddit.com/r/learnpython/comments/qfc29n/i_need_to_make_a_python_application_that_takes_an/
    # https://www.google.com/search?q=bitmap+file&oq=bitmap+file&gs_lcrp=EgRlZGdlKgcIABAAGIAEMgcIABAAGIAEMgcIARAAGIAEMgcIAhAAGIAEMgcIAxAAGIAEMgcIBBAAGIAEMgcIBRAAGIAEMgcIBhAAGIAEMgcIBxAAGIAE0gEIMTIyNGowajSoAgCwAgE&sourceid=chrome&ie=UTF-8
    # https://www.geeksforgeeks.org/python/python-shift-from-front-to-rear-in-list/