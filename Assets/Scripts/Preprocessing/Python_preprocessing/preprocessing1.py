import pandas as pd

FILENAME_IN = "CollisionCheck.csv"
FILENAME_OUT = "CollisionCheck_preprocessed.csv"
IGNORE_PHRASES = [",NONE"]

def delete_NONE_and_count_col():
    n_comma_max = 0
    with open(FILENAME_IN) as fin, open(FILENAME_OUT, "w+") as fout:
        for line in fin:
            for phrase in IGNORE_PHRASES:
                line = line.replace(phrase, "")
            fout.write(line)
            n_comma = line.count(",")
            if n_comma>n_comma_max:
                n_comma_max = n_comma
    n_col_max = n_comma_max + 1
    return n_col_max

def generate_prepend_str(n_col_max):
    prepend_str = "0"
    for i in range(n_col_max-1):
        prepend_str += ",0"
    return prepend_str

def prepend_line(prepend_str):
    with open(FILENAME_OUT, "r+") as f:
        content = f.read()
        f.seek(0, 0)
        f.write(prepend_str + "\n" + content)

def main():
    
    n_col_max = delete_NONE_and_count_col()
    prepend_str = generate_prepend_str(n_col_max)
    # To read the csv file, the first line needs to be the thickest 
    prepend_line(prepend_str)

    df = pd.read_csv(FILENAME_OUT, header=None, dtype=str)
    
    df = df[1:]
    n_row, _ = df.shape

    new_df = [['x'] * (n_col_max-1)] * n_row
    for i in range(n_row):
        new_df[i] = df.iloc[i].to_numpy(dtype=str)[1:]
        for idx, element in enumerate(new_df[i]):
            if element not in ["nan"," NONE","NONE","NONE ","deadend","-1:1","-1:0"]:
                new_df[i][idx] = str(int(float(element)-1))
            if element == " " or element == " NONE" or element == "NONE" or element == "NONE ":
                new_df[i][idx] = "" 
            if element == " " or element == "":
                new_df[i][idx] = "nan"
            
    new_df = pd.DataFrame(new_df)
    new_df.to_csv(FILENAME_OUT, header=None, index=False)

if __name__=="__main__":
    main()
