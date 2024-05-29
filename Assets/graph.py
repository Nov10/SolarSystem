import matplotlib.pyplot as plt
import matplotlib.ticker as ticker

def read_values_from_file(file_path):
    values = []
    with open(file_path, 'r') as file:
        for line in file:
            try:
                values.append(float(line.strip()))
            except ValueError:
                continue
    return values

def plot_values(values):
    plt.plot(values)
    plt.xlabel('Time')
    plt.ylabel('Area')
    plt.title('Area by Kepler 2nd Law')
    plt.grid(True)
    plt.ylim(0, max(values)*1.5)
    
    plt.show()

if __name__ == "__main__":
    file_path = 'D:/UnityProjects/SS/SolorSystem (2)/Assets/SValues.txt'
    values = read_values_from_file(file_path)
    plot_values(values)
