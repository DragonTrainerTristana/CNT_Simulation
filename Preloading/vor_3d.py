import matplotlib.pyplot as plt
import numpy as np
import json
from scipy.spatial import Voronoi

N_DOTS = 30
SPACE_LENGTH = 10
N_DIM = 3

np.random.seed(0)
dots = np.random.uniform(0, SPACE_LENGTH, (N_DOTS, N_DIM))
vor = Voronoi(dots)

np.savetxt("points.csv", vor.points, delimiter=',', fmt='%f')
np.savetxt("voronoi_vertices.csv", vor.vertices, delimiter=',', fmt='%f')
vor_dict = {str(key)[1:-1]: value for key, value in vor.ridge_dict.items()}

with open('voronoi_dict.json', 'w') as vd:
    json.dump(vor_dict, vd)

fig = plt.figure()
ax = fig.add_subplot(projection='3d')
plt.plot(vor.points[:, 0], vor.points[:, 1], vor.points[:, 2], 'ro', ms=5)
plt.plot(vor.vertices[:, 0], vor.vertices[:, 1], vor.vertices[:, 2], 'ko', ms=2)
ax.set_xlim(0, SPACE_LENGTH)
ax.set_ylim(0, SPACE_LENGTH)
ax.set_zlim(0, SPACE_LENGTH)

for point_idx, vertice_idx in zip(vor.ridge_points, vor.ridge_vertices):
    vertice_idx = np.asarray(vertice_idx)
    if np.all(vertice_idx >= 0):
        for i in range(len(vertice_idx) - 1):
            v0 = vor.vertices[vertice_idx[i]]
            v1 = vor.vertices[vertice_idx[i+1]]
            plt.plot([v0[0], v1[0]],
                     [v0[1], v1[1]],
                     [v0[2], v1[2]],
                     'k', linewidth=1)
    else:
        i = vertice_idx[vertice_idx >= 0][0]
        t = vor.points[point_idx[1]] - vor.points[point_idx[0]]  # tangent
        t /= np.linalg.norm(t)

plt.show()
