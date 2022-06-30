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

#helper
def truncate(number, digits) -> float:
    stepper = 10.0 ** digits
    return math.trunc(stepper * number) / stepper

##Print out log-log charts of the input data (used for paradigm spread)
#largely follows from http://www.mkivela.com/binning_tutorial.html
def logLogHistogramOut (data, title, multiplier):

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
        print('ERROR creating', title, 'plot. Run the test longer or change the bin size')
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
    logBinnedX = []
    for b in nBins:
        logBinnedX.append(math.log10(b))
    logValues = []
    for i in range(len(values)):
        if values[i] != 0:
            logValues.append(math.log10(values[i]))
        else:
            logValues.append(math.log10(values[i-1]/2))#the hacky bit
    #remove the first bin value (so only using the right limit of each bin)
    #and remove the last bin value so centered on what tend to be the most representative results
    #CHANGE THESE VALUES to add more specific regression lines for specific tests
    logBinnedX = logBinnedX[2:len(logBinnedX)-1]
    logValues = logValues[1:len(logValues)-1]
    #now get slope of the logged values
    try:
        slope, intercept, r_value, p_value, std_err = stats.linregress(logBinnedX, logValues)
    except:
        print('ERROR creating', title, 'plot. Run the test longer or change the bin size')
        return
    nnBins = nBins[1:len(nBins)-1]
    #in power law form the slope becomes the exponent, and 10 to the intercept becomes the scaling constant
    plot1.plot(nnBins, (10**intercept)*nnBins**slope, 'r', label='fitted line')
    

    #print out the chart
    plot1.set_title('PDF, ' + title + ',\nSlope: '+ str(truncate(slope, 3)) + ' r: ' + str(truncate(r_value, 3)))
    plot1.set_xlim(1, nBins[len(nBins)-1])
    plot1.set_ylim(top = 1)
    fig.savefig(title + 'PDFandSlope.svg')



#MAIN-------------------------------------------------------------------
#first stuff for individual paradigms
maxAdherents = []
totalAdoptions = []
monthsActive = []

with open('Paradigms_Tracking.csv') as csvDataFile:
    csvReader = csv.reader(csvDataFile)
    first = True
    for row in csvReader:
        if first == False:
            maxAdherents.append(int(row[1]))
            row2 = int(row[2]) - 1
            if row2 > 0:#don't count the original leviathan mutation as an "adoption"
                totalAdoptions.append(row2)
            #totalAdoptions.append(int(row[2])) #to include the original mutation as an adoption
            monthsActive.append(int(row[3]))
        else:
            first = False

logLogHistogramOut(maxAdherents, 'Max Followers', 2)
logLogHistogramOut(totalAdoptions, 'Total Adoptions', 2)
logLogHistogramOut(monthsActive, 'Months Active', 2)

################################################################
#WEEKLY
#population!
weeks = []
population = []
yields = []
costs = []

with open('Population_Tracking.csv') as csvDataFile:
    csvReader1 = csv.reader(csvDataFile)
    first = True
    for row in csvReader1:
        if not (row):    
            continue
        if first == False:
            weeks.append(int(row[0]))
            population.append(int(row[1]))
            yields.append(float(row[2]))
            costs.append(float(row[3]))
        else:
            first = False

fig = plt.figure(figsize=(14,30))
plot1 = fig.add_subplot(911)
plot1.grid(b=None, which='both', axis='x')
plot1.plot(weeks, population)
plot1.set_xlim(np.min(weeks), np.max(weeks))
plot1.set_ylim(0)
plot1.set_title('Population and Yields')
plot1.set_xlabel('Week')
plot1.set_ylabel('Total Population of all Leviathans', color = 'C0')
[i.set_color("C0") for i in plot1.get_yticklabels()]

#yields
plo1ax2 = plot1.twinx()
plo1ax2.plot(weeks, yields, 'r')
plo1ax2.set_ylim(0, np.max(yields)*1.5)
plo1ax2.set_ylabel('Total Yield of all Leviathans', color = 'r')
[i.set_color("red") for i in plo1ax2.get_yticklabels()]

#costs vs benefits
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
totalParaChange = []
with open('Change_Tracking.csv') as csvDataFile:
    csvReader2 = csv.reader(csvDataFile)
    first = True
    for row in csvReader2:
        if not (row):    
            continue
        if first == False:
            months.append(int(row[0]))
            totalParaChange.append(int(row[3]))
        else:
            first = False

plot2 = fig.add_subplot(912)
plot2.grid(b=None, which='both', axis='x')
plot2.plot(months, totalParaChange)
plot2.set_xlim(np.min(months), np.max(months))
plot2.set_ylim(0)
plot2.set_title('Paradigm Shifts')
plot2.set_xlabel('Month')
plot2.set_ylabel('Number of Paradigm\nAdoptions and Mutations', color = 'C0')
[i.set_color("C0") for i in plot2.get_yticklabels()]

#yieldMultiplier
months = []
totalYieldChange = []
with open('Yield_Multiplier.csv') as csvDataFile:
    csvReader2 = csv.reader(csvDataFile)
    first = True
    for row in csvReader2:
        if not (row):    
            continue
        if first == False:
            #if int(row[0]) % 4 == 0:
                months.append(int(row[0])/4)
                totalYieldChange.append(float(row[1]))
        else:
            first = False

plo2ax2 = plot2.twinx()
plo2ax2.plot(months, totalYieldChange, 'r')
plo2ax2.set_ylim(8, 16)
plo2ax2.set_ylabel('Yield Multiplier', color = 'r')
[i.set_color("red") for i in plo2ax2.get_yticklabels()]



#largest paradigm and number of paradigms at any one time
largestPara = []
numPara = []
with open('Paradigm_Sizes.csv') as csvDataFile:
    csvReader2 = csv.reader(csvDataFile)
    first = True
    for row in csvReader2:
        if not (row):    
            continue
        if first == False:
            #if int(row[0]) % 4 == 0:
                #months.append(int(row[0])/4)
                largestPara.append(int(row[1]))
                numPara.append(int(row[2]))
        else:
            first = False


#plot largest para
plot3 = fig.add_subplot(913)
plot3.grid(b=None, which='both', axis='x')
plot3.plot(months, numPara)
plot3.set_xlim(np.min(months), np.max(months))
plot3.set_ylim(0)
plot3.set_title('Active Paradigms and Largest Paradigm')
plot3.set_xlabel('Month')
plot3.set_ylabel('Number of Active Paradigms', color = 'C0')
[i.set_color("C0") for i in plot3.get_yticklabels()]

#plot num para
plo3ax2 = plot3.twinx() 
plo3ax2.plot(months, largestPara, 'r')
plo3ax2.set_ylim(0)
plo3ax2.set_ylabel('Most Followers of any Paradigm', color = 'r')
[i.set_color("red") for i in plo3ax2.get_yticklabels()]



#-----average # of rules, average birth rate
avNumRules = []
avBirthRate = []
numGiveSurplus = []
with open('Rates_Tracking.csv') as csvDataFile:
    csvReader2 = csv.reader(csvDataFile)
    first = True
    for row in csvReader2:
        if not (row):    
            continue
        if first == False:
                avNumRules.append(float(row[1]))
                avBirthRate.append(float(row[2]))
        else:
            first = False

#plot average brith rate
plot4 = fig.add_subplot(914)
plot4.grid(b=None, which='both', axis='x')
plot4.plot(months, avBirthRate)
plot4.set_xlim(np.min(months), np.max(months))
plot4.set_ylim(0)
plot4.set_title('Birth rates and Influence Radius')
plot4.set_xlabel('Month')
plot4.set_ylabel('Average Birth Rate', color = 'C0')
[i.set_color("C0") for i in plot4.get_yticklabels()]

#plot avergage # of rules
plot5 = fig.add_subplot(915)
plot5.grid(b=None, which='both', axis='x')
plot5.plot(months, avNumRules)
plot5.set_xlim(np.min(months), np.max(months))
plot5.set_ylim(0)
plot5.set_title('Number of Rules and Average Threshold')
plot5.set_xlabel('Month')
plot5.set_ylabel('Average Number of Rules by Leviathan', color = 'C0')
[i.set_color("C0") for i in plot5.get_yticklabels()]


#--------avAdoptionRate, avMutationDivider, avThreshold
avAdoptionRate = []
avMutationDivider = []
avThreshold = []
with open('MutationRates_Tracking.csv') as csvDataFile:
    csvReader2 = csv.reader(csvDataFile)
    first = True
    for row in csvReader2:
        if not (row):    
            continue
        if first == False:
                avAdoptionRate.append(float(row[1]))
                avMutationDivider.append(float(row[2]))
                avThreshold.append(float(row[3]))
        else:
            first = False

#plot average threshold
plo5ax2 = plot5.twinx()
plo5ax2.plot(months, avThreshold, 'r')
plo5ax2.set_ylim(0)
plo5ax2.set_ylabel('Average Threshold by leviathan', color = 'r')
[i.set_color("red") for i in plo5ax2.get_yticklabels()]

#plot average adoption threshold
plot6 = fig.add_subplot(916)
plot6.grid(b=None, which='both', axis='x')
plot6.plot(months, avAdoptionRate)
plot6.set_xlim(np.min(months), np.max(months))
plot6.set_ylim(0)
plot6.set_title('Adoption Threshold and Mutation Rate')
plot6.set_xlabel('Month')
plot6.set_ylabel('Average Adoption Threshold by Leviathan', color = 'C0')
[i.set_color("C0") for i in plot6.get_yticklabels()]

#plot average mutation rate
plo6ax2 = plot6.twinx()
plo6ax2.plot(months, avMutationDivider, 'r')
plo6ax2.set_ylim(0)
plo6ax2.set_ylabel('Average Mutation Rate by Leviathan', color = 'r')
[i.set_color("red") for i in plo6ax2.get_yticklabels()]



#------average birthCom, deathCom, emmiCom, sensitivity, and influence
avBirthCom = []
avDeathCom = []
avEmmiCom = []
avSensitivity = []
avInfluence = []
with open('Sensitivity_Tracking.csv') as csvDataFile:
    csvReader2 = csv.reader(csvDataFile)
    first = True
    for row in csvReader2:
        if not (row):    
            continue
        if first == False:
                avBirthCom.append(float(row[1]))
                avDeathCom.append(float(row[2]))
                avEmmiCom.append(float(row[3]))
                avSensitivity.append(float(row[4]))
                avInfluence.append(float(row[5]))
        else:
            first = False

#plot avergage birthCom
plot7 = fig.add_subplot(917)
plot7.grid(b=None, which='both', axis='x')
plot7.plot(months, avBirthCom)
plot7.set_xlim(np.min(months), np.max(months))
plot7.set_ylim(0)
plot7.set_title('BirthCom and DeathCom')
plot7.set_xlabel('Month')
plot7.set_ylabel('Average BirthCom by Leviathan', color = 'C0')
[i.set_color("C0") for i in plot7.get_yticklabels()]

#plot average deathCom
plo7ax2 = plot7.twinx()
plo7ax2.plot(months, avDeathCom, 'r')
plo7ax2.set_ylim(0, -50)
plo7ax2.set_ylabel('Average DeathCom by Leviathan', color = 'r')
[i.set_color("red") for i in plo7ax2.get_yticklabels()]

#plot average emmiCom
plot8 = fig.add_subplot(918)
plot8.grid(b=None, which='both', axis='x')
plot8.plot(months, avEmmiCom)
plot8.set_xlim(np.min(months), np.max(months))
plot8.set_ylim(0)
plot8.set_title('EmmiCom and Overal Sensitivity')
plot8.set_xlabel('Month')
plot8.set_ylabel('Average EmmiCom by Leviathan', color = 'C0')
[i.set_color("C0") for i in plot8.get_yticklabels()]

#plot average sensitivity
plo8ax2 = plot8.twinx()
plo8ax2.plot(months, avSensitivity, 'r')
plo8ax2.set_ylim(0)
plo8ax2.set_ylabel('Average Sensitivity by Leviathan', color = 'r')
[i.set_color("red") for i in plo8ax2.get_yticklabels()]

#plot average influence (on same chart as birth rates because space)
plo4ax2 = plot4.twinx()
plo4ax2.plot(months, avInfluence, 'r')
plo4ax2.set_ylim(0)
plo4ax2.set_ylabel('Average Influence Radius', color = 'r')
[i.set_color("red") for i in plo4ax2.get_yticklabels()]



#AND now Average comfort and expectedResults
avComfort = []
avExpects = []
avEmProb = []
avStorage = []
avWorkRate = []
with open('ComfortExpects_Tracking.csv') as csvDataFile:
    csvReader2 = csv.reader(csvDataFile)
    first = True
    for row in csvReader2:
        if not (row):    
            continue
        if first == False:
                avComfort.append(float(row[1]))
                avExpects.append(float(row[2]))
                avEmProb.append(float(row[3]))
                avStorage.append(float(row[4]))
                avWorkRate.append(float(row[5]))
        else:
            first = False

#plot avergage comfortableness
plot9 = fig.add_subplot(919)
plot9.grid(b=None, which='both', axis='x')
plot9.plot(months, avComfort)
plot9.set_xlim(np.min(months), np.max(months))
plot9.set_ylim(0, 100)
plot9.set_title('Comfort and Expectations')
plot9.set_xlabel('Month')
plot9.set_ylabel('Average Comfort by Leviathan', color = 'C0')
[i.set_color("C0") for i in plot9.get_yticklabels()]

#plot average expectedResults
plo9ax2 = plot9.twinx()
plo9ax2.plot(months, avExpects, 'r')
plo9ax2.set_ylim(0)
plo9ax2.set_ylabel('Average Expectations by Leviathan', color = 'r')
[i.set_color("red") for i in plo9ax2.get_yticklabels()]

#plot avergage emProb
fig3 = plt.figure(figsize=(14,6))
plot10 = fig3.add_subplot(211)
plot10.grid(b=None, which='both', axis='x')
plot10.plot(months, avEmProb)
plot10.set_xlim(np.min(months), np.max(months))
plot10.set_ylim(0)
plot10.set_title('EmProb and Storage')
plot10.set_xlabel('Month')
plot10.set_ylabel('Average EmProb', color = 'C0')
[i.set_color("C0") for i in plot10.get_yticklabels()]

#plot average storagewant
plo10ax2 = plot10.twinx()
plo10ax2.plot(months, avStorage, 'r')
plo10ax2.set_ylim(0)
plo10ax2.set_ylabel('Average Storage Want', color = 'r')
[i.set_color("red") for i in plo10ax2.get_yticklabels()]


#plot avergage work rate
plot11 = fig3.add_subplot(212)
plot11.grid(b=None, which='both', axis='x')
plot11.plot(months, avWorkRate)
plot11.set_xlim(np.min(months), np.max(months))
plot11.set_ylim(0)
plot11.set_title('Work rate')
plot11.set_xlabel('Month')
plot11.set_ylabel('Average Work Rate', color = 'C0')
[i.set_color("C0") for i in plot11.get_yticklabels()]
fig3.tight_layout()
fig3.savefig('EMPROBANDSTORAGE.svg')



#########################################################################
#SAVE
fig.tight_layout()
fig.savefig('PopulationandParaidmgChange.svg')
print ('COMPLETE!')

