from PIL import Image as pict
import os

def resize_picture(picture_way, width, height):

    picture = pict.open(picture_way)

    resized_picture = picture.resize((width, height))

    file_name = os.path.basename(picture_way)

    save_way = f"images/resized_{file_name}"

    resized_picture.save(save_way)

    return save_way


def rotate_picture(picture_way, corner):

    picture = pict.open(picture_way)

    rotated_picture = picture.rotate(corner)

    file_name = os.path.basename(picture_way)

    save_way = f"images/rotated_{file_name}"

    rotated_picture.save(save_way)

    return save_way
