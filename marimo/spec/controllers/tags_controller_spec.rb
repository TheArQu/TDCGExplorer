require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe TagsController do
  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  def mock_user(stubs={})
    @_mock_user ||= mock_model(User, stubs)
  end

  def mock_tag(stubs={})
    @_mock_tag ||= mock_model(Tag, stubs)
  end
  
  def mock_arc(stubs={})
    @_mock_arc ||= mock_model(Arc, stubs)
  end
  
  describe "GET index" do
    it "assigns all tags as @tags" do
      Tag.stub!(:paginate).and_return([mock_tag])
      get :index
      assigns[:tags].should == [mock_tag]
    end
  end

  describe "GET show" do
    it "assigns the requested tag as @tag" do
      Tag.stub!(:find).with("37").and_return(mock_tag(:arcs => [ mock_arc ]))
      get :show, :id => "37"
      assigns[:tag].should equal(mock_tag)
    end
  end

  describe "GET new" do
    it "assigns a new tag as @tag" do
      Tag.stub!(:new).and_return(mock_tag)
      get :new
      assigns[:tag].should equal(mock_tag)
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tag.stub!(:new).and_return(mock_tag)
      get :new
    end
  end

  describe "GET edit" do
    it "assigns the requested tag as @tag" do
      Tag.stub!(:find).with("37").and_return(mock_tag)
      get :edit, :id => "37"
      assigns[:tag].should equal(mock_tag)
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tag.stub!(:find).and_return(mock_tag)
      get :edit, :id => "1"
    end
  end

  describe "POST create" do
    
    describe "with valid params" do
      it "assigns a newly created tag as @tag" do
        Tag.stub!(:new).with({'these' => 'params'}).and_return(mock_tag(:save => true))
        post :create, :tag => {:these => 'params'}
        assigns[:tag].should equal(mock_tag)
      end

      it "redirects to the created tag" do
        Tag.stub!(:new).and_return(mock_tag(:save => true))
        post :create, :tag => {}
        response.should redirect_to(tag_url(mock_tag))
      end
    end
    
    describe "with invalid params" do
      it "assigns a newly created but unsaved tag as @tag" do
        Tag.stub!(:new).with({'these' => 'params'}).and_return(mock_tag(:save => false))
        post :create, :tag => {:these => 'params'}
        assigns[:tag].should equal(mock_tag)
      end

      it "re-renders the 'new' template" do
        Tag.stub!(:new).and_return(mock_tag(:save => false))
        post :create, :tag => {}
        response.should render_template('new')
      end
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tag.stub!(:new).and_return(mock_tag(:save => true))
      put :create, :tag => {}
    end
    
  end

  describe "PUT update" do
    
    describe "with valid params" do
      it "updates the requested tag" do
        Tag.should_receive(:find).with("37").and_return(mock_tag)
        mock_tag.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :tag => {:these => 'params'}
      end

      it "assigns the requested tag as @tag" do
        Tag.stub!(:find).and_return(mock_tag(:update_attributes => true))
        put :update, :id => "1"
        assigns[:tag].should equal(mock_tag)
      end

      it "redirects to the tag" do
        Tag.stub!(:find).and_return(mock_tag(:update_attributes => true))
        put :update, :id => "1"
        response.should redirect_to(tag_url(mock_tag))
      end
    end
    
    describe "with invalid params" do
      it "updates the requested tag" do
        Tag.should_receive(:find).with("37").and_return(mock_tag)
        mock_tag.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :tag => {:these => 'params'}
      end

      it "assigns the tag as @tag" do
        Tag.stub!(:find).and_return(mock_tag(:update_attributes => false))
        put :update, :id => "1"
        assigns[:tag].should equal(mock_tag)
      end

      it "re-renders the 'edit' template" do
        Tag.stub!(:find).and_return(mock_tag(:update_attributes => false))
        put :update, :id => "1"
        response.should render_template('edit')
      end
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tag.stub!(:find).and_return(mock_tag(:update_attributes => true))
      put :update, :id => "1"
    end
    
  end

  describe "DELETE destroy" do
    it "destroys the requested tag" do
      Tag.should_receive(:find).with("37").and_return(mock_tag)
      mock_tag.should_receive(:destroy)
      delete :destroy, :id => "37"
    end
  
    it "redirects to the tags list" do
      Tag.stub!(:find).and_return(mock_tag(:destroy => true))
      delete :destroy, :id => "1"
      response.should redirect_to(tags_url)
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tag.stub!(:find).and_return(mock_tag)
      mock_tag.stub!(:destroy).and_return(true)
      delete :destroy, :id => "1"
    end
  end

end
