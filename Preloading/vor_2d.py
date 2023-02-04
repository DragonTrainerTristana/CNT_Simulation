import numpy as np
import matplotlib.pyplot as plt
from scipy.spatial import Voronoi, voronoi_plot_2d

N_DOTS = 10
SPACE_LENGTH = 10
N_DIM = 2

def main():
    np.random.seed(0)
    dots = np.random.uniform(0, SPACE_LENGTH,
                             (N_DOTS, N_DIM))
    vor = Voronoi(dots)

    print(vor.ridge_dict)
    print("\n")
    print(vor.ridge_points)
    print("\n")
    print(vor.ridge_vertices)

    # fig = voronoi_plot_2d(vor, show_vertices=False)
    fig = plt.figure()
    plt.plot(vor.points[:, 0], vor.points[:, 1], 'ko', ms=5)
    for point_idx, vpair in zip(vor.ridge_points, vor.ridge_vertices):
        vpair = np.asarray(vpair)
        if np.all(vpair >= 0):
            v0 = vor.vertices[vpair[0]]
            v1 = vor.vertices[vpair[1]]
            # Draw a line from v0 to v1.
            plt.plot([v0[0], v1[0]], [v0[1], v1[1]], 'k', linewidth=1)
        else:
            i = vpair[vpair >= 0][0]
            t = vor.points[point_idx[1]] - vor.points[point_idx[0]]  # tangent
            t /= np.linalg.norm(t)
            n = np.array([-t[1], t[0]])  # normal
            # print(t, n)
            midpoint = vor.points[point_idx].mean(axis=0)
            direction = np.sign(np.dot(midpoint - vor.points.mean(axis=0), n)) * n
            far_point = vor.vertices[i] + direction * vor.points.ptp(axis=0).max()
            plt.plot([vor.vertices[i][0], far_point[0]],
                     [vor.vertices[i][1], far_point[1]], 'k--', linewidth=1)
    plt.xlim(0, SPACE_LENGTH)
    plt.ylim(0, SPACE_LENGTH)
    plt.show()

if __name__=="__main__":
    main()
