require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Relationship do
  fixtures :arcs, :relationships

  it "one は関係性を持つ" do
    arcs(:one).relationships.should == [ relationships(:one) ]
  end

  it "one は two と関係を持つ" do
    arcs(:one).relations.should == [ arcs(:two) ]
  end

  it "two は逆関係性を持つ" do
    arcs(:two).rev_relationships.should == [ relationships(:one) ]
  end

  it "two は one と逆関係を持つ" do
    arcs(:two).rev_relations.should == [ arcs(:one) ]
  end
end
