from matplotlib import pyplot as plt
import io
from matplotlib.backends.backend_agg import FigureCanvasAgg
from PIL import Image


def plt_fig_to_pil(fig: plt.figure):
    canvas = FigureCanvasAgg(fig)
    buf = io.BytesIO()
    canvas.print_png(buf)
    buf.seek(0)
    img = Image.open(buf)
    return img
