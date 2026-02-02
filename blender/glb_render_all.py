import bpy, os

dirpath = '/home/bex/Documents/blender/hexagonset_renders'

def set_render_self_and_children(glb, show):
    glb.hide_render = not show
    #print("  setting " + str(glb) + " to " + str(show))
    #print("  children: " + str(glb.children))
    for child in glb.children:
        child.hide_render = not show
        #print("  setting " + str(child) + " to " + str(show))

glbs = list(bpy.data.collections["GLBs"].objects)

object = None

for glb in glbs:
    if glb.parent != None:
        continue
    #print("rendering " + str(glb))
    set_render_self_and_children(glb, True)
    
    for other in glbs:
        if glb == other:
            continue
        if other.parent != None:
            continue
        #print("checking " + str(other))
        set_render_self_and_children(other, False)
    
    bpy.context.scene.render.filepath = os.path.join(dirpath, glb.name + '.png')
    
    bpy.ops.render.render(animation=False, write_still=True)