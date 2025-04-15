import cv2 as cv
import numpy as np
import matplotlib.pyplot as plt

def read_img(path, img_type = None):
    if img_type == 'color':
        type = None
    elif img_type == 'gray':
        type = cv.IMREAD_GRAYSCALE

    if(img_type):
        return cv.imread(path, type)
    return cv.imread(path)

def img_add(img, value):
    return np.clip(img.astype(np.int32) + value, 0, 255).astype(np.uint8)

def img_substract(img, value):
    return np.clip(img.astype(np.int32) - value, 0, 255).astype(np.uint8)

def img_multiply(img, value):
    return np.clip(img.astype(np.int32) * value, 0, 255).astype(np.uint8)

def img_divide(img, value):
    return np.clip(img.astype(np.int32) / value, 0, 255).astype(np.uint8)

operations = {
    'add': img_add,
    'substract': img_substract,
    'multiply': img_multiply,
    'divide': img_divide
}