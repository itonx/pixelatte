import cv2 as cv
import numpy as np
import matplotlib.pyplot as plt
from os import path

def read_img(img_path, img_type = None):
    if img_type == 'color':
        type = None
    elif img_type == 'gray':
        type = cv.IMREAD_GRAYSCALE

    _, file_extension = path.splitext(img_path) 
    
    if(img_type):
        return cv.imread(img_path, type), file_extension
    
    return cv.imread(img_path), file_extension

def img_add(img, value):
    return np.clip(img.astype(np.int32) + value, 0, 255).astype(np.uint8)

def img_substract(img, value):
    return np.clip(img.astype(np.int32) - value, 0, 255).astype(np.uint8)

def img_multiply(img, value):
    print(img)
    print(np.clip(img.astype(np.uint32) * value, 0, 255).astype(np.uint8))
    return np.clip(img.astype(np.uint32) * value, 0, 255).astype(np.uint8)

def img_divide(img, value):
    return np.clip(img.astype(np.int32) / value, 0, 255).astype(np.uint8)

operations = {
    'add': img_add,
    'substract': img_substract,
    'multiply': img_multiply,
    'divide': img_divide
}