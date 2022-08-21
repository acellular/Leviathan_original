#####For analysis of the .csvs output by leviathan running in the Unity engine###################
##---------(press space in the Unity game window while running to output)---------
print ("Hello")

import csv
import matplotlib.pyplot as plt
import matplotlib as mpl
mpl.rcParams['figure.dpi'] = 300
import numpy as np
import math
from scipy import stats

##Print out log-log charts of the input data (used for paradigm spread)
#largely follows from http://www.mkivela.com/binning_tutorial.html
def log_log_histogram (data, title, multiplier):

    #set up chart
    fig = plt.figure(figsize=(6, 6))
    plot1 = fig.add_subplot(111)
    plot1.set_xscale('log')
    plot1.set_yscale('log',nonpositive='clip')
    plot1.set_xlabel('x')
    plot1.set_ylabel('PDF')

    # create log bins: (by specifying the multiplier)
    try:
        bins = [np.min(data)]
        cur_value = bins[0]
    except:
        print('ERROR creating bins for', title, 'plot. Run the test longer or try changing the bin size')
        return
        
    while cur_value < np.max(data):
        cur_value = cur_value * multiplier
        bins.append(cur_value)

    #print basic histogram
    bins = np.array(bins)
    values, nBins, patches = plot1.hist(data, bins=bins, density=True)
    
    #add regression line
    #NOTE!!!!---a bit HACKY when there's no data in a bin
    #just takes previous point and averages it with nothing
    #--better to change the bin size
    #or the values that regression covers
    #or simply run longer tests resulting in more data and thus less chance of empty bins
    log_binnedX = []
    for b in nBins:
        log_binnedX.append(math.log10(b))
    log_values = []
    for i in range(len(values)):
        if values[i] != 0:
            log_values.append(math.log10(values[i]))
        else:
            log_values.append(math.log10(values[i-1]/2))#the hacky bit
    #remove the first bin value (so only using the right limit of each bin)
    #and remove the last bin value so centered on what tend to be the most representative results
    #CHANGE THESE VALUES to add more specific regression lines for specific tests
    log_binnedX = log_binnedX[2:len(log_binnedX)-1]
    log_values = log_values[1:len(log_values)-1]
    #now get slope of the logged values
    try:
        slope, intercept, r_value, p_value, std_err = stats.linregress(log_binnedX, log_values)
    except:
        print('ERROR calculating regression line on', title, 'plot. Run the test longer or try changing the bin size')
        return
    nnBins = nBins[1:len(nBins)-1]
    #in power law form the slope becomes the exponent, and 10 to the intercept becomes the scaling constant
    plot1.plot(nnBins, (10**intercept)*nnBins**slope, 'r')
    

    #print out the chart
    plot1.set_title(f'PDF, {title}\nSlope: {slope:.3f} r: {r_value:.3f}')
    plot1.set_xlim(1, nBins[len(nBins)-1])
    plot1.set_ylim(top = 1)
    fig.savefig(title + 'PDFandSlope.svg')


def add_plot(fig, subplot, x, y, x_label, y_label, title):
    plot = fig.add_subplot(subplot)
    plot.grid(b=None, which='both', axis='x')
    plot.plot(x, y)
    plot.set_xlim(np.min(x), np.max(x))
    plot.set_ylim(0, np.max(y)*1.1)
    plot.set_title(title)
    plot.set_xlabel(x_label)
    plot.set_ylabel(y_label, color = 'C0')
    for i in plot.get_yticklabels(): i.set_color("C0")
    return plot


def add_axis(plot, x, y, y_label, y_lim_mod=1.05):
    axis = plot.twinx()
    axis.plot(x, y, 'r')
    axis.set_ylim(0, np.max(y)*y_lim_mod)
    axis.set_ylabel(y_label, color = 'r')
    for i in axis.get_yticklabels(): i.set_color("red")
    return axis


# collect data and create charts
# TODO additional refacoring in tandem with CSV output from main Unity program
# (no reason anymore to have seperate CSVs or plotting calls)
if __name__ == "__main__":

    # POWER LAW analysis of paradigm spread
    max_adherents = []
    total_adoptions = []
    months_active = []

    with open('Paradigms_Tracking.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        first = True
        for row in csv_reader:
            if first == False:
                max_adherents.append(int(row[1]))
                row2 = int(row[2]) - 1
                if row2 > 0:#don't count the original leviathan mutation as an "adoption"
                    total_adoptions.append(row2)
                #totalAdoptions.append(int(row[2])) #to include the original mutation as an adoption
                months_active.append(int(row[3]))
            else:
                first = False

    log_log_histogram(max_adherents, 'Max Followers', 2)
    log_log_histogram(total_adoptions, 'Total Adoptions', 2)
    log_log_histogram(months_active, 'Months Active', 2)


    #### SYSTEM EVOLUTION CHARTS (i.e. population, yields, number of paradigms, average num rules per leviathan)
    fig = plt.figure(figsize=(14,30))

    ################################################################
    #WEEKLY
    #population!
    weeks = []
    population = []
    yields = []
    costs = []

    with open('Population_Tracking.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        next(csv_reader , None)  # skip the header
        for row in csv_reader:
            if row:
                weeks.append(int(row[0]))
                population.append(int(row[1]))
                yields.append(float(row[2]))
                costs.append(float(row[3]))

    plot1 = add_plot(fig, 911, weeks, population, 'Week', 'Total Population of all Leviathans', 'Population and Yields')
    add_axis(plot1, weeks, yields, 'Total Yield of all Leviathans', y_lim_mod=1.5)


    #### EXTRA for costs vs benefits
    plot1.plot(weeks, costs, 'g')
    fig2 = plt.figure(figsize=(11,5.5))
    plot21 = fig2.add_subplot(121)
    weeks2 = np.array(weeks)*(1/len(weeks))
    c = np.tan(weeks2)

    plot21.set_title('Cost-Benefit "Seneca" Curve')
    plot21.set_xlabel('Paradigm Costs')
    plot21.set_ylabel('Yields')
    plot21.scatter(costs[10::5], yields[10::5], c=c[10::5], marker='.')

    plot22 = fig2.add_subplot(122)
    plot22.set_title('Costs vs Benefits')
    plot22.set_xlabel('Paradigm Rule Costs')
    plot22.set_ylabel('Population')
    plot22.scatter(costs[10::5], population[10::5], c=c[10::5], marker='.')

    fig2.tight_layout(pad=2, w_pad=2, h_pad=0)
    fig2.savefig('CostBenefitCurve.png')


    ############################################################################
    #MONTHLY
    #number of adoptions or mutations (paradigm shifts)
    months = []
    total_para_change = []

    with open('Change_Tracking.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        next(csv_reader , None)  # skip the header
        for row in csv_reader:
            if row:
                months.append(int(row[0]))
                total_para_change.append(int(row[3]))

    #yieldMultiplier
    yield_mult = []
    with open('Yield_Multiplier.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        next(csv_reader , None)  # skip the header
        for row in csv_reader:
            if row:
                yield_mult.append(float(row[1]))

    plot2 = add_plot(fig, 912, months, total_para_change, 'Month', 'Number of Paradigm\nAdoptions and Mutations', 'Paradigm Shifts')
    add_axis(plot2, months, yield_mult, 'Yield Multiplier', y_lim_mod=1.5)


    #largest paradigm and number of paradigms at any one time
    largest_para = []
    num_para = []
    with open('Paradigm_Sizes.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        next(csv_reader , None)  # skip the header
        for row in csv_reader:
            if row:
                largest_para.append(int(row[1]))
                num_para.append(int(row[2]))

    plot3 = add_plot(fig, 913, months, largest_para, 'Month', 'Most Followers of any Paradigm', 'Active Paradigms and Largest Paradigm')
    add_axis(plot3, months, num_para, 'Number of Active Paradigms')


    #-----average # of rules, average birth rate
    avg_num_rules = []
    avg_birth_rate = []
    with open('Rates_Tracking.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        next(csv_reader , None)  # skip the header
        for row in csv_reader:
            if row:
                avg_num_rules.append(float(row[1]))
                avg_birth_rate.append(float(row[2]))

    plot4 = add_plot(fig, 914, months, avg_birth_rate, 'Month', 'Average Birth Rate', 'Birth rates and Influence Radius')
    plot5 = add_plot(fig, 915, months, avg_num_rules, 'Month', 'Average Number of Rules by Leviathan', 'Number of Rules and Average Threshold')


    #------average adoption threshold, mutation rate, and base threshold
    avg_adopt_thresh = []
    avg_mut_rate = []
    avg_thresh = []
    with open('MutationRates_Tracking.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        next(csv_reader , None)  # skip the header
        for row in csv_reader:
            if row:
                avg_adopt_thresh.append(float(row[1]))
                avg_mut_rate.append(float(row[2]))
                avg_thresh.append(float(row[3]))

    add_axis(plot5, months, avg_thresh, 'Average Threshold by leviathan')
    plot6 = add_plot(fig, 916, months, avg_adopt_thresh, 'Month', 'Average Adoption Threshold by Leviathan', 'Adoption Threshold and Mutation Rate')
    add_axis(plot6, months, avg_mut_rate, 'Average Mutation Rate by Leviathan')


    #-----average comfor influence from births, deaths, emmigation, plus sensitivity, and influence
    # approximately where CSVs started getting real messy
    avg_birth_comfort = []
    avg_death_comfort = []
    avg_emmigration_comfort = []
    avg_sensitivity = []
    avg_influence = []
    with open('Sensitivity_Tracking.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        next(csv_reader , None)  # skip the header
        for row in csv_reader:
            if row:
                avg_birth_comfort.append(float(row[1]))
                avg_death_comfort.append(float(row[2]))
                avg_emmigration_comfort.append(float(row[3]))
                avg_sensitivity.append(float(row[4]))
                avg_influence.append(float(row[5]))

    plot7 = add_plot(fig, 917, months, avg_birth_comfort, 'Month', 'Average BirthCom by Leviathan', 'BirthCom and DeathCom')
    axis7b = add_axis(plot7, months, avg_death_comfort, 'Average DeathCom by Leviathan')
    axis7b.set_ylim(np.min(avg_death_comfort)-5, 0)

    plot8 = add_plot(fig, 918, months, avg_emmigration_comfort, 'Month', 'Average EmmiCom by Leviathan', 'EmmiCom and Overal Sensitivity')
    add_axis(plot8, months, avg_sensitivity, 'Average Sensitivity by Leviathan')

    add_axis(plot4, months, avg_influence, 'Average Influence Radius')


    #AND now Average comfort and expectedResults and MORE!
    avg_comfort = []
    avg_expects = []
    avg_emProb = []
    avg_storage = []
    avg_workRate = []
    with open('ComfortExpects_Tracking.csv') as csv_file:
        csv_reader = csv.reader(csv_file)
        next(csv_reader , None)  # skip the header
        for row in csv_reader:
            if row:
                avg_comfort.append(float(row[1]))
                avg_expects.append(float(row[2]))
                avg_emProb.append(float(row[3]))
                avg_storage.append(float(row[4]))
                avg_workRate.append(float(row[5]))

    plot9 = add_plot(fig, 919, months, avg_comfort, 'Month', 'Average Comfort by Leviathan', 'Comfort and Expectations')
    add_axis(plot9, months, avg_expects, 'Average Expectations by Leviathan')

    #### extra chart for emProb, storage and work rate
    fig3 = plt.figure(figsize=(14,6))
    plot10 = add_plot(fig3, 211, months, avg_emProb, 'Month', 'Average EmProb', 'EmProb and Storage')
    add_axis(plot10, months, avg_storage, 'Average Storage Want')

    plot11 = add_plot(fig3, 212, months, avg_workRate, 'Month', 'Average Work rate', 'Work Rate')

    
    #####-------SAVE----------
    fig.tight_layout()
    fig.savefig('evolution_charts.svg')
    fig3.tight_layout()
    fig3.savefig('evolution_charts_additional.svg')

    print ('COMPLETE!')