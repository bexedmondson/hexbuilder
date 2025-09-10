import bpy, bmesh


def set_render_self_and_children(glb, show):
    glb.hide_render = not show
    for child in glb.children:
        child.hide_render = not show

glbs = list(bpy.data.collections["GLBs"].all_objects)

for glb in glbs:
    glbdata = glbs[1].data

    bm = bmesh.new()
    bm.from_mesh(glbdata)
    bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=0.0001)
    bm.to_mesh(glbdata)
    bm.free()
