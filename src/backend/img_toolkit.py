import cv2 as cv
import numpy as np
import matplotlib.pyplot as plt
from os import path
from random import randrange

def read_img(img_path, img_type = None):
    if img_type == 'color':
        type = cv.IMREAD_COLOR_BGR
    elif img_type == 'gray':
        type = cv.IMREAD_GRAYSCALE

    _, file_extension = path.splitext(img_path) 
    
    img_result = cv.imread(img_path, type)
    print(img_result.shape)
    return img_result, file_extension, img_result.shape[1], img_result.shape[0]

def img_add(img, value):
    return np.clip(img.astype(np.int32) + value, 0, 255).astype(np.uint8)

def img_substract(img, value):
    return np.clip(img.astype(np.int32) - value, 0, 255).astype(np.uint8)

def img_multiply(img, value):
    return np.clip(img.astype(np.uint32) * value, 0, 255).astype(np.uint8)

def img_divide(img, value):
    return np.clip(img.astype(np.int32) / value, 0, 255).astype(np.uint8)

operations = {
    'add': img_add,
    'substract': img_substract,
    'multiply': img_multiply,
    'divide': img_divide
}

def get_img_with_salt_and_pepper(img, width, height, noise_level, img_type = None):
    if(noise_level > 50): noise_level = 50
    random_threshold = np.random.randint(0, 100, size=(height, width))
    img[random_threshold < noise_level] = 0 if img_type == 'gray' else np.zeros((3), np.uint8)
    img[random_threshold > (100-noise_level)] = 255 if img_type == 'gray' else np.full((3), 255, np.uint8)
    return img