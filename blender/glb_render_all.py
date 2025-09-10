import bpy, os

dirpath = '/home/bex/Documents/blender/hexagonset_renders'

def set_render_self_and_children(glb, show):
    glb.hide_render = not show
    for child in glb.children:
        child.hide_render = not show

glbs = list(bpy.data.collections["GLBs"].all_objects)

for glb in glbs:
    set_render_self_and_children(glb, True)
    
    for other in glbs:
        if glb == other:
            continue
        set_render_self_and_children(other, False)
    
    bpy.context.scene.render.filepath = os.path.join(dirpath, glb.name + '.png')
    
    bpy.ops.render.render(animation=False, write_still=True)